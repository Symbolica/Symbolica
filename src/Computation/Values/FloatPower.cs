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

    public override FPExpr AsFloat(IContext context)
    {
        throw new UnsupportedSymbolicArithmeticException();
    }

    public RealExpr AsReal(IContext context)
    {
        ArithExpr ToArithExpr(IValue value) =>
            value switch
            {
                IRealValue r => r.AsReal(context),
                _ => context.CreateExpr(c =>
                {
                    using var flt = value.AsFloat(context);
                    return c.MkFPToReal(flt);
                })
            };

        return context.CreateExpr(c =>
        {
            using var left = ToArithExpr(_left);
            using var right = ToArithExpr(_right);
            return (RealExpr) c.MkPower(left, right);
        });
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantSingle(MathF.Pow(l, r)),
            (l, r) => new ConstantDouble(Math.Pow(l, r)),
            (l, r) => new FloatPower(l, r));
    }
}
