using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    internal class MkFPLEq : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, FPExpr> _t1;
        private readonly IFunc<Context, FPExpr> _t2;

        public MkFPLEq(IFunc<Context, FPExpr> t1, IFunc<Context, FPExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BoolExpr Run(Context ctx) => ctx.MkFPLEq(_t1.Run(ctx), _t2.Run(ctx));
    }
}
