using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkFPToBV : IFunc<Context, BitVecExpr>
    {
        private readonly IFunc<Context, FPRMNum> _rm;
        private readonly IFunc<Context, FPExpr> _t;
        private readonly uint _sz;
        private readonly bool _signed;

        public MkFPToBV(IFunc<Context, FPRMNum> rm, IFunc<Context, FPExpr> t, uint sz, bool signed = false)
        {
            _rm = rm;
            _t = t;
            _sz = sz;
            _signed = signed;
        }

        public BitVecExpr Run(Context ctx) =>
            ctx.MkFPToBV(_rm.Run(ctx), _t.Run(ctx), _sz, _signed);
    }
}
