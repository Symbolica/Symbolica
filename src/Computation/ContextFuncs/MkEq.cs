using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkEq : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, Expr> _left;
        private readonly IFunc<Context, Expr> _right;

        public MkEq(IFunc<Context, Expr> left, IFunc<Context, Expr> right)
        {
            _left = left;
            _right = right;
        }

        public BoolExpr Run(Context ctx) =>
            ctx.MkEq(_left.Run(ctx), _right.Run(ctx));
    }
}
