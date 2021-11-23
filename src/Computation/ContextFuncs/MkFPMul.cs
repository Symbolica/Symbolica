using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkFPMul : IFunc<Context, FPExpr>
    {
        private readonly IFunc<Context, FPRMExpr> _rm;
        private readonly IFunc<Context, FPExpr> _t1;
        private readonly IFunc<Context, FPExpr> _t2;

        public MkFPMul(IFunc<Context, FPRMExpr> rm, IFunc<Context, FPExpr> t1, IFunc<Context, FPExpr> t2)
        {
            _rm = rm;
            _t1 = t1;
            _t2 = t2;
        }

        public FPExpr Run(Context ctx) => ctx.MkFPMul(_rm.Run(ctx), _t1.Run(ctx), _t2.Run(ctx));
    }
}
