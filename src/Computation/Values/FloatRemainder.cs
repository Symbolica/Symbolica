using System;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record FloatRemainder : Float
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatRemainder(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override FPExpr AsFloat(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var left = _left.AsFloat(context);
            using var right = _right.AsFloat(context);
            return c.MkFPRem(left, right);
        });
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantSingle(MathF.IEEERemainder(l, r)),
            (l, r) => new ConstantDouble(Math.IEEERemainder(l, r)),
            (l, r) => new FloatRemainder(l, r));
    }
}
