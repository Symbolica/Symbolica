using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    [Serializable]
    internal class MkFPRNE : IFunc<Context, FPRMNum>
    {
        public FPRMNum Run(Context ctx) => ctx.MkFPRNE();
    }
}
