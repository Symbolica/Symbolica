using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkFPRoundToIntegral : IFunc<Context, FPExpr>
    {
        private readonly IFunc<Context, FPRMExpr> _rm;
        private readonly IFunc<Context, FPExpr> _t;

        public MkFPRoundToIntegral(IFunc<Context, FPRMExpr> rm, IFunc<Context, FPExpr> t)
        {
            _rm = rm;
            _t = t;
        }

        public FPExpr Run(Context ctx) => ctx.MkFPRoundToIntegral(_rm.Run(ctx), _t.Run(ctx));
    }
}
