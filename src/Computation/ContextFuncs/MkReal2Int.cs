using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    internal class MkReal2Int : IFunc<Context, IntExpr>
    {
        private readonly IFunc<Context, RealExpr> _t;

        public MkReal2Int(IFunc<Context, RealExpr> t)
        {
            _t = t;
        }

        public IntExpr Run(Context ctx) => ctx.MkReal2Int(_t.Run(ctx));
    }
}
