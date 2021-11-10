using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    internal class MkPower<TExpr> : IFunc<Context, TExpr>
        where TExpr : ArithExpr
    {
        private readonly IFunc<Context, ArithExpr> _t1;
        private readonly IFunc<Context, ArithExpr> _t2;

        public MkPower(IFunc<Context, ArithExpr> t1, IFunc<Context, ArithExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public TExpr Run(Context ctx) => (TExpr)ctx.MkPower(_t1.Run(ctx), _t2.Run(ctx));
    }
}
