using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkInt2BV : IFunc<Context, BitVecExpr>
    {
        private readonly uint _n;
        private readonly IFunc<Context, IntExpr> _t;

        public MkInt2BV(uint n, IFunc<Context, IntExpr> t)
        {
            _n = n;
            _t = t;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkInt2BV(_n, _t.Run(ctx));
    }
}
