using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    internal class MkXor : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, BoolExpr> _left;
        private readonly IFunc<Context, BoolExpr> _right;

        public MkXor(IFunc<Context, BoolExpr> left, IFunc<Context, BoolExpr> right)
        {
            _left = left;
            _right = right;
        }

        public BoolExpr Run(Context ctx) =>
            ctx.MkXor(_left.Run(ctx), _right.Run(ctx));
    }
}
