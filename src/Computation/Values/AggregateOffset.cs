using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class AggregateOffset : Integer
{
    private bool? _isBounded;
    private readonly ImmutableList<(BigInteger, IValue)> _restOffsets;

    private AggregateOffset(
        IValue baseAddress,
        IValue offset,
        BigInteger aggregateSize,
        ImmutableList<(BigInteger, IValue)> offsets,
        bool? isBounded)
        : base(baseAddress.Size)
    {
        AggregateSize = aggregateSize;
        BaseAddress = baseAddress;
        Offset = offset;
        _restOffsets = offsets;
        _isBounded = isBounded;
    }

    public BigInteger AggregateSize { get; }

    public IValue BaseAddress { get; }

    public IValue Offset { get; }

    private IEnumerable<(BigInteger, IValue)> AllOffsets => new[] { (AggregateSize, Offset) }.Concat(_restOffsets);

    public override IEnumerable<IValue> Children => new[] { BaseAddress }.Concat(AllOffsets.Select(x => x.Item2));

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
        return AllOffsets
            .Aggregate(BaseAddress, (l, r) => Values.Add.Create(l, r.Item2));
    }

    internal bool IsBounded(IAssertions assertions, Bits valueSize)
    {
        IValue OffsetIsBounded(AggregateOffset aggregateOffset)
        {
            var fieldSize = aggregateOffset.GetNext()?.AggregateSize ?? (uint) valueSize;
            return And.Create(
                SignedGreaterOrEqual.Create(
                    aggregateOffset.Offset,
                    ConstantUnsigned.Create(aggregateOffset.Offset.Size, BigInteger.Zero)),
                SignedLessOrEqual.Create(
                    Values.Add.Create(aggregateOffset.Offset, ConstantUnsigned.Create(aggregateOffset.Offset.Size, fieldSize)),
                    ConstantUnsigned.Create(aggregateOffset.Offset.Size, aggregateOffset.AggregateSize)));
        }

        IEnumerable<AggregateOffset> Offsets(AggregateOffset? offset)
        {
            while (offset is not null)
            {
                yield return offset;
                offset = offset.GetNext();
            }
        }

        if (_isBounded is not null)
        {
            return _isBounded.Value;
        }

        var isContained = Offsets(this).Aggregate(
            new ConstantBool(true) as IValue,
            (acc, o) => And.Create(acc, OffsetIsBounded(o)));
        using var proposition = assertions.GetProposition(isContained);
        _isBounded = !proposition.CanBeFalse;
        // if (!_isBounded.Value)
        //     Debugger.Break();
        return _isBounded.Value;
    }

    internal IValue Add(IConstantValue value)
    {
        return Create(
            Values.Add.Create(BaseAddress, value),
            AllOffsets,
            _isBounded);
    }

    internal IValue Multiply(IConstantValue value)
    {
        return Create(
            Values.Multiply.Create(BaseAddress, value),
            AllOffsets.Select(o => (value.AsUnsigned() * o.Item1, Values.Multiply.Create(o.Item2, value))),
            _isBounded);
    }

    internal IValue Subtract(IValue value)
    {
        return Create(
            Values.Subtract.Create(BaseAddress, value),
            AllOffsets,
            _isBounded);
    }

    internal AggregateOffset Negate()
    {
        static IValue NegateValue(IValue value) => Values.Multiply.Create(value, ConstantUnsigned.Create(value.Size, -1));
        return Create(
            NegateValue(BaseAddress),
            AllOffsets.Select(o => (o.Item1, NegateValue(o.Item2))),
            _isBounded);
    }

    internal AggregateOffset? GetNext()
    {
        return _restOffsets.Count > 0
            ? Create(ConstantUnsigned.Create(BaseAddress.Size, 0), _restOffsets, _isBounded)
            : null;
    }

    private static AggregateOffset Create(IValue baseAddress, IEnumerable<(BigInteger, IValue)> offsets, bool? isBounded)
    {
        AggregateOffset Merge(AggregateOffset baseAddress)
        {
            return Create(baseAddress.BaseAddress, baseAddress.AllOffsets.Concat(offsets), isBounded);
        }

        // bool OffsetIsDegenerate((BigInteger, IValue) offset, (BigInteger, IValue) prevOffset)
        // {
        //     bool IsZero(IValue value) => value is IConstantValue c && c.AsUnsigned().IsZero;
        //     return offset.Item1 == prevOffset.Item1 && IsZero(offset.Item2);
        // }

        if (!offsets.Any())
        {
            throw new Exception($"{nameof(AggregateOffset)} must have at least one offset.");
        }

        Debug.Assert(!(baseAddress is IConstantValue ba && ba.AsUnsigned() < BigInteger.Zero));

        if (offsets.Any(o => o.Item2 is AggregateOffset))
            Debugger.Break();

        var firstOffset = offsets.First();
        var restOffsets = offsets.Skip(1);

        // var normalisedOffsets = restOffsets.Aggregate(
        //     (Enumerable.Empty<(BigInteger, IValue)>(), firstOffset),
        //     (acc, x) => OffsetIsDegenerate(x, acc.firstOffset)
        //         ? (acc.Item1, x)
        //         : (acc.Item1.Append(x), x));

        return baseAddress switch
        {
            AggregateOffset b => Merge(b),
            _ => new AggregateOffset(
                    baseAddress,
                    firstOffset.Item2,
                    firstOffset.Item1,
                    ImmutableList.CreateRange(restOffsets),
                    isBounded)
        };
    }

    public static AggregateOffset Create(IValue baseAddress, IEnumerable<(BigInteger, IValue)> offsets)
    {
        return Create(baseAddress, offsets, null);
    }
}
