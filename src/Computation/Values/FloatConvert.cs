using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record FloatConvert : Float
{
    private readonly IValue _value;

    private FloatConvert(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override ISet<IValue> Symbols => _value.Symbols;

    public override FPExpr AsFloat(ISolver solver)
    {
        using var rounding = solver.Context.MkFPRNE();
        using var value = _value.AsFloat(solver);
        using var sort = Size.GetSort(solver);
        return solver.Context.MkFPToFP(rounding, value, sort);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as FloatConvert);
    }

    public static IValue Create(Bits size, IValue value)
    {
        return Unary(value,
            v => (uint) size switch
            {
                32U => new ConstantSingle(v),
                64U => new ConstantDouble(v),
                _ => new FloatConvert(size, new ConstantSingle(v))
            },
            v => (uint) size switch
            {
                32U => new ConstantSingle((float) v),
                64U => new ConstantDouble(v),
                _ => new FloatConvert(size, new ConstantDouble(v))
            },
            v => v is IRealValue r
                ? new RealConvert(size, r)
                : new FloatConvert(size, v));
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is FloatConvert v
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
            Size.GetHashCode(),
            _value.GetEquivalencyHash(includeSubs));
    }
}
