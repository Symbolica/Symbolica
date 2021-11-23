using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkFPRTZ : IFunc<Context, FPRMNum>
    {
        public FPRMNum Run(Context ctx) => ctx.MkFPRTZ();
    }
}
