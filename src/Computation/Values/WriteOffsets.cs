using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal record struct WriteOffset
{
    public WriteOffset(Bits aggregateSize, string aggregateType, Bits fieldSize, IValue value)
    {
        if (aggregateSize < fieldSize)
            throw new Exception("Can't create a WriteOffset with an aggregateSize smaller than the fieldSize.");

        AggregateSize = aggregateSize;
        AggregateType = aggregateType;
        FieldSize = fieldSize;
        Value = value;
    }

    public Bits AggregateSize { get; }

    public string AggregateType { get; }

    public Bits FieldSize { get; }

    public IValue Value { get; }

    internal bool IsBoundedBy(IAssertions assertions, IValue offset, Bits size)
    {
        var isBounded = And.Create(
            SignedGreaterOrEqual.Create(Value, offset),
            SignedLessOrEqual.Create(
                Add.Create(Value, ConstantUnsigned.Create(Value.Size, (uint) FieldSize)),
                Add.Create(offset, ConstantUnsigned.Create(offset.Size, (uint) size))));
        using var proposition = assertions.GetProposition(isBounded);
        return !proposition.CanBeFalse();
    }

    internal bool IsZero(IAssertions assertions)
    {
        var isZero = Equal.Create(Value, ConstantUnsigned.Zero(Value.Size));
        using var proposition = assertions.GetProposition(isZero);
        return !proposition.CanBeFalse();
    }

    internal WriteOffset Subtract(Bits aggregateSize, IValue offset)
    {
        return new WriteOffset(aggregateSize, AggregateType, FieldSize, Values.Subtract.Create(Value, offset));
    }
}

internal sealed class WriteOffsets
{
    private readonly IEnumerable<WriteOffset> _offsets;

    private WriteOffsets(IEnumerable<WriteOffset> offsets)
    {
        _offsets = offsets;
    }

    public bool Empty => !_offsets.Any();

    public WriteOffset Head() => _offsets.First();

    public WriteOffsets Tail()
    {
        return new WriteOffsets(_offsets.Skip(1));
    }

    public WriteOffsets Rebase(Bits aggregateSize, IValue offset)
    {
        return new WriteOffsets(new[] { Head().Subtract(aggregateSize, offset) }.Concat(Tail()._offsets));
    }

    public static WriteOffsets Create(IValue offset, Bits bufferSize, Bits valueSize)
    {
        IEnumerable<WriteOffset> CreateFromAddress(Address<Bits> address)
        {
            var baseOffset =
                new Offset<Bits>(
                    bufferSize,
                    "Base buffer",
                    address.Offsets
                        .Where(o => o.AggregateType != "Pointer")
                        .Select(o => o.AggregateSize)
                        .FirstOrDefault(valueSize),
                    address.BaseAddress);

            Debug.Assert(address.Offsets.Skip(1).Zip(address.Offsets.SkipLast(1)).All(x => x.First.AggregateSize == x.Second.FieldSize));

            return address.Offsets.Aggregate(
                new List<Offset<Bits>> { baseOffset },
                (acc, offset) =>
                {
                    var prevOffset = acc[^1];
                    if (offset.AggregateType == "Pointer")
                    {
                        // Disabled until loads of addresses perform a bitcast
                        // if (prevOffset.FieldSize != offset.FieldSize)
                        //     throw new InconsistentExpressionSizesException(prevOffset.FieldSize, offset.FieldSize);

                        acc[^1] = new Offset<Bits>(
                            prevOffset.AggregateSize,
                            prevOffset.AggregateType,
                            prevOffset.FieldSize,
                            Add.Create(prevOffset.Value, offset.Value));
                        return acc;
                    }
                    acc.Add(offset);
                    return acc;
                }).Select(o => new WriteOffset(o.AggregateSize, o.AggregateType, o.FieldSize, o.Value));
        }

        var offsets = offset switch
        {
            Address<Bits> a => CreateFromAddress(a),
            Address<Bytes> => throw new Exception($"Cannot create Offsets from an {nameof(Address<Bytes>)}."),
            _ => new[] { new WriteOffset(bufferSize, "Artificial Aggregate", valueSize, offset) }
        };

        Debug.Assert(offsets.Skip(1).Zip(offsets.SkipLast(1)).All(x => x.First.AggregateSize == x.Second.FieldSize));

        return new WriteOffsets(offsets);
    }
}
