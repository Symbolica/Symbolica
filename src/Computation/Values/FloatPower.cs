using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class FloatPower : Float, IRealValue
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatPower(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<IValue> Children => new[] { _left, _right };

    public override string? PrintedValue => null;

    public override FPExpr AsFloat(IContext context)
    {
        throw new UnsupportedSymbolicArithmeticException();
    }

    public RealExpr AsReal(IContext context)
    {
        using var left = _left is IRealValue l
            ? l.AsReal(context)
            : context.CreateExpr(c => c.MkFPToReal(_left.AsFloat(context)));

        using var right = _right is IRealValue r
            ? r.AsReal(context)
            : context.CreateExpr(c => c.MkFPToReal(_right.AsFloat(context)));

        return context.CreateExpr(c => (RealExpr) c.MkPower(left, right));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantSingle(MathF.Pow(l, r)),
            (l, r) => new ConstantDouble(Math.Pow(l, r)),
            (l, r) => new FloatPower(l, r));
    }
}
