using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    internal class MkExtract : IFunc<Context, BitVecExpr>
    {
        private readonly uint _high;
        private readonly uint _low;
        private readonly IFunc<Context, BitVecExpr> _t;

        public MkExtract(uint high, uint low, IFunc<Context, BitVecExpr> t)
        {
            _high = high;
            _low = low;
            _t = t;
        }

        public BitVecExpr Run(Context ctx) => ctx.MkExtract(_high, _low, _t.Run(ctx));
    }
}
