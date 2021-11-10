using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    internal class MkFPNeg : IFunc<Context, FPExpr>
    {
        private readonly IFunc<Context, FPExpr> _t;

        public MkFPNeg(IFunc<Context, FPExpr> t)
        {
            _t = t;
        }

        public FPExpr Run(Context ctx) => ctx.MkFPNeg(_t.Run(ctx));
    }
}
