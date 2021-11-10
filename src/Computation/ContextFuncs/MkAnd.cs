using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    internal class MkAnd : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, BoolExpr> _left;
        private readonly IFunc<Context, BoolExpr> _right;

        public MkAnd(IFunc<Context, BoolExpr> left, IFunc<Context, BoolExpr> right)
        {
            _left = left;
            _right = right;
        }

        public BoolExpr Run(Context ctx) =>
            ctx.MkAnd(_left.Run(ctx), _right.Run(ctx));
    }
}
