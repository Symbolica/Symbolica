using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkITE<TExpr> : IFunc<Context, TExpr>
        where TExpr : Expr
    {
        private readonly IFunc<Context, BoolExpr> _predicate;
        private readonly IFunc<Context, Expr> _left;
        private readonly IFunc<Context, Expr> _right;

        public MkITE(IFunc<Context, BoolExpr> predicate, IFunc<Context, Expr> left, IFunc<Context, Expr> right)
        {
            _left = left;
            _right = right;
            _predicate = predicate;
        }

        public TExpr Run(Context ctx) =>
            (TExpr)ctx.MkITE(_predicate.Run(ctx), _left.Run(ctx), _right.Run(ctx));
    }
}
