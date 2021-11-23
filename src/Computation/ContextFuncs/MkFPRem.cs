using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkFPRem : IFunc<Context, FPExpr>
    {
        private readonly IFunc<Context, FPExpr> _t1;
        private readonly IFunc<Context, FPExpr> _t2;

        public MkFPRem(IFunc<Context, FPExpr> t1, IFunc<Context, FPExpr> t2)
        {
            _t1 = t1;
            _t2 = t2;
        }

        public FPExpr Run(Context ctx) => ctx.MkFPRem(_t1.Run(ctx), _t2.Run(ctx));
    }
}
