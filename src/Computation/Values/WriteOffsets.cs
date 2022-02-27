using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal record struct WriteOffset(Bits AggregateSize, Bits FieldSize, IValue Value)
{
    public bool IsDegenerate()
    {
        return FieldSize == AggregateSize && Value is IConstantValue c && c.AsUnsigned().IsZero;
    }

    public IValue IsBounded()
    {
        return And.Create(
            SignedGreaterOrEqual.Create(
                Value,
                ConstantUnsigned.Create(Value.Size, BigInteger.Zero)),
            SignedLessOrEqual.Create(
                Add.Create(Value, ConstantUnsigned.Create(Value.Size, (uint) FieldSize)),
                ConstantUnsigned.Create(Value.Size, (uint) AggregateSize)));
    }
}

internal sealed class WriteOffsets : Integer
{
    private readonly IEnumerable<WriteOffset> _offsets;

    private WriteOffsets(Bits size, IEnumerable<WriteOffset> offsets)
        : base(size)
    {
        _offsets = offsets;
    }

    public bool Empty => !_offsets.Any();


    public override IEnumerable<IValue> Children => _offsets.Select(x => x.Value);

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(IContext context)
    {
        return Aggregate().AsBitVector(context);
    }

    public override BoolExpr AsBool(IContext context)
    {
        return Aggregate().AsBool(context);
    }

    internal IValue Aggregate()
    {
        return _offsets
            .Aggregate(
                ConstantUnsigned.Create(Size, BigInteger.Zero) as IValue,
                (l, r) => Add.Create(l, r.Value));
    }

    public WriteOffset Head() => _offsets.First();

    public WriteOffsets Tail()
    {
        return new WriteOffsets(Size, _offsets.Skip(1));
    }

    public static WriteOffsets Create(IAssertions assertions, IValue offset, Bits bufferSize, Bits valueSize)
    {
        IEnumerable<WriteOffset> CreateFromAddress(Address<Bits> address)
        {
            var fieldSizes = address.Offsets.Select(o => o.AggregateSize).Append(valueSize);
            var offsets = new[] { new Offset<Bits>(bufferSize, address.BaseAddress) }.Concat(address.Offsets);

            var writeOffsets = offsets
                .Zip(fieldSizes)
                .Select(o => new WriteOffset(o.First.AggregateSize, o.Second, o.First.Value));

            var isContained = writeOffsets.Aggregate(
                new ConstantBool(true) as IValue,
                (acc, o) => And.Create(acc, o.IsBounded()));
            using var proposition = assertions.GetProposition(isContained);

            return proposition.CanBeFalse
                ? new[] { new WriteOffset(bufferSize, valueSize, address.Aggregate()) }
                : writeOffsets;
        }

        if (offset is WriteOffsets wo)
        {
            return wo;
        }

        var offsets = offset switch
        {
            Address<Bits> a => CreateFromAddress(a),
            Address<Bytes> => throw new Exception($"Cannot create Offsets from an {nameof(Address<Bytes>)}."),
            _ => new[] { new WriteOffset(bufferSize, valueSize, offset) }
        };

        var normalisedOffsets = offsets
            .Where(x => !x.IsDegenerate());

        return new WriteOffsets(offset.Size, normalisedOffsets);
    }
}
