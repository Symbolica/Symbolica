using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record FloatPower : Float, IRealValue
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatPower(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override FPExpr AsFloat(ISolver solver)
    {
        throw new UnsupportedSymbolicArithmeticException();
    }

    public RealExpr AsReal(ISolver solver)
    {
        using var left = _left is IRealValue l
            ? l.AsReal(solver)
            : AsReal(solver, _left);

        using var right = _right is IRealValue r
            ? r.AsReal(solver)
            : AsReal(solver, _right);

        return (RealExpr) solver.Context.MkPower(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as FloatPower);
    }

    private static RealExpr AsReal(ISolver solver, IValue value)
    {
        using var flt = value.AsFloat(solver);
        return solver.Context.MkFPToReal(flt);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantSingle(MathF.Pow(l, r)),
            (l, r) => new ConstantDouble(Math.Pow(l, r)),
            (l, r) => new FloatPower(l, r));
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is FloatPower v
             ? _left.IsEquivalentTo(v._left)
                 .And(_right.IsEquivalentTo(v._right))
             : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : Create(_left.Substitute(subs), _right.Substitute(subs));
    }

    public override object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Left = _left.ToJson(),
            Right = _right.ToJson()
        };
    }

    public override int GetEquivalencyHash()
    {
        return HashCode.Combine(GetType().Name, _left.GetEquivalencyHash(), _right.GetEquivalencyHash());
    }
}
