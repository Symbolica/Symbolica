using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkFPIsNaN : IFunc<Context, BoolExpr>
    {
        private readonly IFunc<Context, FPExpr> _expr;

        public MkFPIsNaN(IFunc<Context, FPExpr> expr)
        {
            _expr = expr;
        }

        public BoolExpr Run(Context ctx) => ctx.MkFPIsNaN(_expr.Run(ctx));
    }
}
