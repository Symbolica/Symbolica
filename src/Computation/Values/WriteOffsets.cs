using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal record struct WriteOffset(Bits AggregateSize, string AggregateType, Bits FieldSize, IValue Value)
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

internal sealed record WriteOffsets : Integer
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

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        return Aggregate().AsBitVector(solver);
    }

    public override BoolExpr AsBool(ISolver solver)
    {
        return Aggregate().AsBool(solver);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as WriteOffsets);
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

    public static WriteOffsets Create(ISolver solver, IValue offset, Bits bufferSize, Bits valueSize)
    {
        if (bufferSize == (Bits) 156160
            && valueSize == (Bits) 32
            && offset is Address<Bits> addr
            && addr.BaseAddress is IConstantValue ba
            && (BigInteger) ba.AsUnsigned() == 0
            && addr.Offsets.Count() == 9
            && addr.Offsets.ElementAt(3).Value is IConstantValue c3
            && (BigInteger) c3.AsUnsigned() == 1600)
            Debugger.Break();
        IEnumerable<WriteOffset> CreateFromAddress(Address<Bits> address)
        {
            var baseOffset =
                new Offset<Bits>(
                    bufferSize,
                    "Base buffer",
                    address.Offsets.Select(o => o.AggregateSize).First(),
                    address.BaseAddress);

            Debug.Assert(address.Offsets.Skip(1).Zip(address.Offsets.SkipLast(1)).All(x => x.First.AggregateSize == x.Second.FieldSize));

            var widenedArrayPointers = address.Offsets.Aggregate(
                new List<Offset<Bits>> { baseOffset },
                (acc, offset) =>
                {
                    var prevOffset = acc[^1];
                    if (offset.AggregateType == "Pointer" && prevOffset.AggregateType == "Array")
                    {
                        acc[^1] = new Offset<Bits>(
                            prevOffset.AggregateSize,
                            prevOffset.AggregateType,
                            prevOffset.FieldSize,
                            Add.Create(prevOffset.Value, offset.Value));
                        return acc;
                    }
                    acc.Add(offset);
                    return acc;
                }).ToList();

            var finalOffset = widenedArrayPointers.Last();

            var writeOffsets = widenedArrayPointers
                .Select(o => new WriteOffset(o.AggregateSize, o.AggregateType, o.FieldSize, o.Value))
                .Append(new WriteOffset(finalOffset.FieldSize, "Value", valueSize, ConstantUnsigned.Zero(finalOffset.Value.Size)));

            var isContained = writeOffsets.Aggregate(
                new ConstantBool(true) as IValue,
                (acc, o) => And.Create(acc, o.IsBounded()));

            return solver.IsSatisfiable(Not.Create(isContained))
                ? new[] { new WriteOffset(bufferSize, "Aggregated - Not bounded", valueSize, address.Aggregate()) }
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
            _ => new[] { new WriteOffset(bufferSize, "Artificial Aggregate", valueSize, offset) }
        };

        Debug.Assert(offsets.Skip(1).Zip(offsets.SkipLast(1)).All(x => x.First.AggregateSize == x.Second.FieldSize));

        var normalisedOffsets = offsets
            .Where(x => !x.IsDegenerate());

        return new WriteOffsets(offset.Size, normalisedOffsets);
    }
}
