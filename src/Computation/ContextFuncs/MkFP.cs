using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkFP : IFunc<Context, FPNum>
    {
        private readonly double _v;
        private readonly IFunc<Context, FPSort> _s;

        public MkFP(double v, IFunc<Context, FPSort> s)
        {
            _v = v;
            _s = s;
        }

        public FPNum Run(Context ctx) => ctx.MkFP(_v, _s.Run(ctx));
    }
}
