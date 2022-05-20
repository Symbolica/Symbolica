using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record FloatNegate : Float
{
    private readonly IValue _value;

    private FloatNegate(IValue value)
        : base(value.Size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(ISolver solver)
    {
        using var value = _value.AsFloat(solver);
        return solver.Context.MkFPNeg(value);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as FloatNegate);
    }

    public static IValue Create(IValue value)
    {
        return Unary(value,
            v => new ConstantSingle(-v),
            v => new ConstantDouble(-v),
            v => new FloatNegate(v));
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is FloatNegate v
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

    public override int GetEquivalencyHash()
    {
        return HashCode.Combine(GetType().Name, _value.GetEquivalencyHash());
    }
}
