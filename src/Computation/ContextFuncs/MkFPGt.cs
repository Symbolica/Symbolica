using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    internal class MkFPGt : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, FPExpr> _t1;
        private readonly IFunc<Context, FPExpr> _t2;

        public MkFPGt(IFunc<Context, FPExpr> t1, IFunc<Context, FPExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public BoolExpr Run(Context ctx) => ctx.MkFPGt(_t1.Run(ctx), _t2.Run(ctx));
    }
}
