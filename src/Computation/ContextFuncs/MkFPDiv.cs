using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    internal class MkFPDiv : IFunc<Context, FPExpr>
    {
        private readonly IFunc<Context, FPRMExpr> _rm;
        private readonly IFunc<Context, FPExpr> _t1;
        private readonly IFunc<Context, FPExpr> _t2;

        public MkFPDiv(IFunc<Context, FPRMExpr> rm, IFunc<Context, FPExpr> t1, IFunc<Context, FPExpr> t2)
        {
            _rm = rm;
            _t1 = t1;
            _t2 = t2;
        }

        public FPExpr Run(Context ctx) => ctx.MkFPDiv(_rm.Run(ctx), _t1.Run(ctx), _t2.Run(ctx));
    }
}
