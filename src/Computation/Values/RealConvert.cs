using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record RealConvert : Float, IRealValue
{
    private readonly IRealValue _value;

    public RealConvert(Bits size, IRealValue value)
        : base(size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(ISolver solver)
    {
        throw new UnsupportedSymbolicArithmeticException();
    }

    public RealExpr AsReal(ISolver solver)
    {
        return _value.AsReal(solver);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as RealConvert);
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is RealConvert v
            ? _value.IsEquivalentTo(v._value)
                .And((new(), Size == v.Size))
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : this;
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
}
