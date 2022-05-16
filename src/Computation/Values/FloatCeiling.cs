using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record FloatCeiling : Float
{
    private readonly IValue _value;

    private FloatCeiling(IValue value)
        : base(value.Size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(ISolver solver)
    {
        using var rounding = solver.Context.MkFPRTP();
        using var value = _value.AsFloat(solver);
        return solver.Context.MkFPRoundToIntegral(rounding, value);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as FloatCeiling);
    }

    public static IValue Create(IValue value)
    {
        return Unary(value,
            v => new ConstantSingle(MathF.Ceiling(v)),
            v => new ConstantDouble(Math.Ceiling(v)),
            v => new FloatCeiling(v));
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is FloatCeiling v
            ? _value.IsEquivalentTo(v._value)
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : Create(_value.Substitute(subs));
    }
}
