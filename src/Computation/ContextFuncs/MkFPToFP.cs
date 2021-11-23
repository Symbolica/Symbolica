using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkFPToFPOfFP : IFunc<Context, FPExpr>
    {
        private readonly IFunc<Context, FPRMNum> _rm;
        private readonly IFunc<Context, FPExpr> _t;
        private readonly IFunc<Context, FPSort> _s;

        public MkFPToFPOfFP(IFunc<Context, FPRMNum> rm, IFunc<Context, FPExpr> t, IFunc<Context, FPSort> s)
        {
            _rm = rm;
            _t = t;
            _s = s;
        }

        public FPExpr Run(Context ctx) =>
            ctx.MkFPToFP(_rm.Run(ctx), _t.Run(ctx), _s.Run(ctx));
    }

    internal class MkFPToFPOfBitVec : IFunc<Context, FPExpr>
    {
        private readonly IFunc<Context, BitVecExpr> _bv;
        private readonly IFunc<Context, FPSort> _s;

        public MkFPToFPOfBitVec(IFunc<Context, BitVecExpr> t, IFunc<Context, FPSort> s)
        {
            _bv = t;
            _s = s;
        }

        public FPExpr Run(Context ctx) => ctx.MkFPToFP(_bv.Run(ctx), _s.Run(ctx));
    }

    internal class MkFPToFPOfFPRMNumBitVec : IFunc<Context, FPExpr>
    {
        private readonly IFunc<Context, FPRMNum> _rm;
        private readonly IFunc<Context, BitVecExpr> _t;
        private readonly IFunc<Context, FPSort> _s;
        private readonly bool _signed;

        public MkFPToFPOfFPRMNumBitVec(IFunc<Context, FPRMNum> rm, IFunc<Context, BitVecExpr> t, IFunc<Context, FPSort> s, bool signed)
        {
            _rm = rm;
            _t = t;
            _s = s;
            _signed = signed;
        }

        public FPExpr Run(Context ctx) =>
            ctx.MkFPToFP(_rm.Run(ctx), _t.Run(ctx), _s.Run(ctx), _signed);
    }
}
