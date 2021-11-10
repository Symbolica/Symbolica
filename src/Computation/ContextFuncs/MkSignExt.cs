using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    internal class MkSignExt : IFunc<Context, BitVecExpr>
    {
        private readonly uint _i;
        private readonly IFunc<Context, BitVecExpr> _t;

        public MkSignExt(uint i, IFunc<Context, BitVecExpr> t)
        {
            _i = i;
            _t = t;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkSignExt(_i, _t.Run(ctx));
    }
}
