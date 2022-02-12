using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class SymbolicFloat : Float
{
    private readonly Func<Context, FPExpr> _func;

    private SymbolicFloat(Bits size, Func<Context, FPExpr> func)
        : base(size)
    {
        _func = func;
    }

    public override FPExpr AsFloat(IContext context)
    {
        return context.CreateExpr(_func);
    }

    public static SymbolicFloat Create(float value)
    {
        return new SymbolicFloat((Bits) 32U, c => c.MkFP(value, c.MkFPSort32()));
    }

    public static SymbolicFloat Create(double value)
    {
        return new SymbolicFloat((Bits) 64U, c => c.MkFP(value, c.MkFPSort64()));
    }
}
