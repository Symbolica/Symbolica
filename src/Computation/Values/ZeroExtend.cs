using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record ZeroExtend : BitVector
{
    private readonly IValue _value;

    private ZeroExtend(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override ISet<IValue> Symbols => _value.Symbols;

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var value = _value.AsBitVector(solver);
        return solver.Context.MkZeroExt((uint) (Size - _value.Size), value);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as ZeroExtend);
    }

    public static IValue Create(Bits size, IValue value)
    {
        return size > value.Size
            ? value is IConstantValue v
                ? v.AsUnsigned().Extend(size)
                : new ZeroExtend(size, value)
            : value;
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is ZeroExtend v
            ? _value.IsEquivalentTo(v._value)
                .And((new(), Size == v.Size))
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : Create(Size, _value.Substitute(subs));
    }

    public override object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Value = _value.ToJson()
        };
    }

    public override int GetEquivalencyHash(bool includeSubs)
    {
        return HashCode.Combine(
            GetType().Name,
            Size,
            _value.GetEquivalencyHash(includeSubs));
    }
}
