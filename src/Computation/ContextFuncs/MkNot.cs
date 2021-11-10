using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    internal class MkNot : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, BoolExpr> _expr;

        public MkNot(IFunc<Context, BoolExpr> expr)
        {
            _expr = expr;
        }

        public BoolExpr Run(Context ctx) => ctx.MkNot(_expr.Run(ctx));
    }
}
