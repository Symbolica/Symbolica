using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record Not : BitVector
{
    private readonly IValue _value;

    private Not(IValue value)
        : base(value.Size)
    {
        _value = value;
    }

    public override ISet<IValue> Symbols => _value.Symbols;

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var value = _value.AsBitVector(solver);
        return solver.Context.MkBVNot(value);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as Not);
    }

    public static IValue Create(IValue value)
    {
        return value switch
        {
            IConstantValue v => v.AsUnsigned().Not(),
            Not v => v._value,
            _ => new Not(value)
        };
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is Not v
            ? _value.IsEquivalentTo(v._value)
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : Create(_value.Substitute(subs));
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
            _value.GetEquivalencyHash(includeSubs));
    }
}
