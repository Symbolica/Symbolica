using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkFPToReal : IFunc<Context, RealExpr>
    {
        private readonly IFunc<Context, FPExpr> _t;

        public MkFPToReal(IFunc<Context, FPExpr> t)
        {
            _t = t;
        }

        public RealExpr Run(Context ctx) => ctx.MkFPToReal(_t.Run(ctx));
    }
}
