using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record AggregateOffset : Integer
{
    private bool? _isBounded = null;
    private readonly ImmutableList<(BigInteger, IValue)> _restOffsets;

    private AggregateOffset(IValue baseAddress, IValue offset, BigInteger aggregateSize, ImmutableList<(BigInteger, IValue)> offsets)
        : base(baseAddress.Size)
    {
        AggregateSize = aggregateSize;
        BaseAddress = baseAddress;
        Offset = offset;
        _restOffsets = offsets;
    }

    public BigInteger AggregateSize { get; }

    public IValue BaseAddress { get; }

    public IValue Offset { get; }

    private IEnumerable<(BigInteger, IValue)> AllOffsets => new[] { (AggregateSize, Offset) }.Concat(_restOffsets);

    public override IEnumerable<IValue> Children => new[] { BaseAddress }.Concat(AllOffsets.Select(x => x.Item2));

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
        return Equals(other as AggregateOffset);
    }

    internal IValue Aggregate()
    {
        return AllOffsets
            .Aggregate(BaseAddress, (l, r) => Add.Create(l, r.Item2));
    }

    internal bool IsBounded(ISolver solver, IValue value)
    {
        IValue OffsetIsBounded(BigInteger size, IValue offset)
        {
            return And.Create(
                UnsignedGreaterOrEqual.Create(
                    offset,
                    ConstantUnsigned.Create(offset.Size, 0)),
                UnsignedLessOrEqual.Create(
                    Add.Create(offset, ConstantUnsigned.Create(offset.Size, (uint) value.Size)),
                    ConstantUnsigned.Create(offset.Size, (uint) size)));
        }
        if (_isBounded is not null)
        {
            return _isBounded.Value;
        }

        var isContained = AllOffsets.Aggregate(
            new ConstantBool(true) as IValue,
            (acc, o) => And.Create(acc, OffsetIsBounded(o.Item1, o.Item2)));

        _isBounded = !solver.IsSatisfiable(Not.Create(isContained));
        return _isBounded.Value;
    }

    internal IValue Multiply(IConstantValue value)
    {
        return Create(
            Values.Multiply.Create(BaseAddress, value),
            ImmutableList.CreateRange(
                AllOffsets.Select(o => (value.AsUnsigned() * o.Item1, Values.Multiply.Create(o.Item2, value)))));
    }

    internal IValue Subtract(IValue value)
    {
        return Create(
            Values.Subtract.Create(BaseAddress, value),
            AllOffsets);
    }

    internal AggregateOffset? GetNext()
    {
        return _restOffsets.Count > 1
            ? Create(ConstantUnsigned.Create(BaseAddress.Size, 0), _restOffsets)
            : null;
    }

    public static AggregateOffset Create(IValue baseAddress, IEnumerable<(BigInteger, IValue)> offsets)
    {
        if (!offsets.Any())
        {
            throw new Exception($"{nameof(AggregateOffset)} must have at least one offset.");
        }
        return new AggregateOffset(
            baseAddress,
            offsets.First().Item2,
            offsets.First().Item1,
            ImmutableList.CreateRange(offsets.Skip(1)));
    }
}
