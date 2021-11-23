using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkZeroExt : IFunc<Context, BitVecExpr>
    {
        private readonly uint _i;
        private readonly IFunc<Context, BitVecExpr> _t;

        public MkZeroExt(uint i, IFunc<Context, BitVecExpr> t)
        {
            _i = i;
            _t = t;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkZeroExt(_i, _t.Run(ctx));
    }
}
