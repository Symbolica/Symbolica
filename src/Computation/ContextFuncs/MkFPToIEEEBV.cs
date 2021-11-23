using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkFPToIEEEBV : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, FPExpr> _t;

        public MkFPToIEEEBV(IFunc<Context, FPExpr> t)
        {
            _t = t;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkFPToIEEEBV(_t.Run(ctx));
    }
}
