using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class AggregateWrite : BitVector
{
    private readonly ImmutableList<AggregateWrite> _fields;

    private readonly IValue _buffer;

    private AggregateWrite(IValue buffer, IValue offset, ImmutableList<AggregateWrite> fields)
        : base(buffer.Size)
    {
        _buffer = buffer;
        Offset = offset;
        _fields = fields;
    }

    public IValue Offset { get; }

    public override IEnumerable<IValue> Children => new[] { _buffer }.Concat(_fields);

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(IContext context)
    {
        return Flatten().AsBitVector(context);
    }

    // public int CompareTo(AggregateWrite? other)
    // {
    //     return (Offset, other?.Offset) switch
    //     {
    //         (IConstantValue x, IConstantValue y) => ((BigInteger) x.AsUnsigned()).CompareTo(y.AsUnsigned()),
    //         (IConstantValue, _) => -1,
    //         (_, _) => 1,
    //     };
    // }

    internal IValue Read(ICollectionFactory collectionFactory, IAssertions assertions,
        AggregateOffset aggregateOffset, Bits valueSize)
    {
        var fieldOffset = aggregateOffset.Offset;
        var fieldSize = (Bits) (uint) aggregateOffset.AggregateSize;

        IValue ReadFieldBuffer(IValue buffer)
        {
            return Values.Read.Create(
                collectionFactory,
                assertions,
                buffer,
                ConstantUnsigned.Create(buffer.Size, 0),
                valueSize);
        }

        IValue ReadSubAggregate(AggregateWrite match)
        {
            var nextOffset = aggregateOffset.GetNext();
            return nextOffset is not null
                ? match.Read(collectionFactory, assertions, nextOffset, valueSize)
                : ReadFieldBuffer(match.Flatten());
        }

        // Check for alignment with constant offsets
        //      Read sub aggregate
        var constantMatch = _fields
            .Where(f => f.Offset is IConstantValue)
            .FirstOrDefault(f => f.IsAligned(assertions, fieldSize, fieldOffset));
        if (constantMatch is not null)
        {
            return ReadSubAggregate(constantMatch);
        }

        // Check for disjunction with everything
        //      Read below
        if (IsNotOverlappingAnyField(assertions, fieldSize, fieldOffset))
        {
            return ReadFieldBuffer(CreateFieldBuffer(_buffer, fieldOffset, fieldSize));
        }

        // Check for alignment with symbolic offsets
        //      Do an overwrite
        var symbolicMatch = _fields
            .Where(f => f.Offset is not IConstantValue)
            .FirstOrDefault(f => f.IsAligned(assertions, fieldSize, fieldOffset));
        if (symbolicMatch is not null)
        {
            return ReadSubAggregate(symbolicMatch);
        }

        // Flatten
        return Values.Read.Create(collectionFactory, assertions, Flatten(), fieldOffset, valueSize);
    }

    // Assumes that aggregateOffset has already been checked to make sure all offset are in bounds
    internal AggregateWrite Write(IAssertions assertions, AggregateOffset aggregateOffset, IValue value)
    {
        var fieldOffset = aggregateOffset.Offset;
        var fieldSize = (Bits) (uint) aggregateOffset.AggregateSize;

        AggregateWrite Overwrite(AggregateWrite match)
        {
            var nextOffset = aggregateOffset.GetNext();
            var newField = nextOffset is not null
                ? match.Write(assertions, nextOffset, value)
                : CreateLeaf(match.Size, Offset, value);
            return new AggregateWrite(_buffer, Offset, _fields.Replace(match, newField));
        }

        // Check for alignment with constant offsets
        //      Do an overwrite
        var constantMatch = _fields
            .Where(f => f.Offset is IConstantValue)
            .FirstOrDefault(f => f.IsAligned(assertions, fieldSize, fieldOffset));
        if (constantMatch is not null)
        {
            return Overwrite(constantMatch);
        }

        // Check for disjunction with everything
        //      Do an insert
        if (IsNotOverlappingAnyField(assertions, fieldSize, fieldOffset))
        {
            return new AggregateWrite(
                _buffer,
                Offset,
                _fields.Add(Create(_buffer, aggregateOffset, value)));
        }

        // Check for alignment with symbolic offsets
        //      Do an overwrite
        var symbolicMatch = _fields
            .Where(f => f.Offset is not IConstantValue)
            .FirstOrDefault(f => f.IsAligned(assertions, fieldSize, fieldOffset));
        if (symbolicMatch is not null)
        {
            return Overwrite(symbolicMatch);
        }

        // Flatten
        return new AggregateWrite(
            Flatten(),
            fieldOffset,
            ImmutableList.Create(Create(_buffer, aggregateOffset, value)));
    }

    private static IValue Mask(Bits bufferSize, IValue offset, Bits size)
    {
        return ShiftLeft.Create(
            ConstantUnsigned.Create(size, BigInteger.Zero).Not().Extend(bufferSize),
            ZeroExtend.Create(bufferSize, offset));
    }

    private bool IsNotOverlappingAnyField(IAssertions assertions, Bits size, IValue offset)
    {
        var isOverlapping = And.Create(AggregateMask(), Mask(_buffer.Size, offset, size));
        using var proposition = assertions.GetProposition(isOverlapping);

        return !proposition.CanBeTrue;
    }

    private bool IsAligned(IAssertions assertions, Bits size, IValue offset)
    {
        if (Size != size)
        {
            return false;
        }
        var isAligned = Equal.Create(Offset, offset);
        using var proposition = assertions.GetProposition(isAligned);

        return !proposition.CanBeFalse;
    }

    private IValue CreateMask()
    {
        return Mask(_buffer.Size, Offset, Size);
    }

    private IValue AggregateFields(Func<AggregateWrite, IValue> f)
    {
        return _fields.Aggregate(
            ConstantUnsigned.Create(Size, BigInteger.Zero) as IValue,
            (acc, field) => Or.Create(acc, f(field)));
    }

    private IValue AggregateMask() => AggregateFields(f => f.CreateMask());

    private IValue Flatten()
    {
        // TODO: Try to separate the rolling up of the values from the application to
        // the underlying buffer so that we can avoid duplication of the underlying.
        // Imagine the case where the underlying is symbolic and we've created sub-portions
        // of it at each layer. Then rolling them all back up could include many copies
        // of this complex expression.
        // Instead, if we keep every single write (even constants) in a layer and then
        // only merge those together for a given a layer, the caller of this can then
        // apply the result to their copy of the underlying which will mean we only see
        // a single instance of it but with a minimised size still.
        IValue CreateData(AggregateWrite field)
        {
            return ShiftLeft.Create(
                ZeroExtend.Create(Size, field.Flatten()),
                ZeroExtend.Create(Size, Offset));
        }

        var data = AggregateFields(CreateData);

        return Or.Create(And.Create(_buffer, Not.Create(AggregateMask())), data);
    }

    private static IValue CreateFieldBuffer(IValue buffer, IValue offset, Bits size)
    {
        return Truncate.Create(
            size,
            LogicalShiftRight.Create(
                buffer,
                ZeroExtend.Create(buffer.Size, offset)));
    }

    public static AggregateWrite Create(IValue buffer, AggregateOffset aggregateOffset, IValue value)
    {
        var fieldOffset = aggregateOffset.Offset;
        var fieldSize = (Bits) (uint) aggregateOffset.AggregateSize;
        var fieldBuffer = CreateFieldBuffer(buffer, fieldOffset, fieldSize);
        var nextOffset = aggregateOffset.GetNext();

        return nextOffset is null
            ? CreateLeaf(fieldSize, fieldOffset, value)
            : new AggregateWrite(
                fieldBuffer,
                fieldOffset,
                ImmutableList.Create(Create(fieldBuffer, nextOffset, value)));
    }

    private static AggregateWrite CreateLeaf(Bits size, IValue offset, IValue value)
    {
        // TODO: Can probably in the general case just call Write.Create here instead of throwing
        // like we do in Read by flattening this AggregateWrite and writing the new value on top of it
        // to create a new buffer for this level.
        return new AggregateWrite(
            value.Size == size
                ? value
                : throw new InconsistentExpressionSizesException(size, value.Size),
            offset,
            ImmutableList.Create<AggregateWrite>());
    }
}
