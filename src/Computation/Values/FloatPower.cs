using System;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values.Constants;

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
}
