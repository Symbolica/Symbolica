using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.ContextFuncs
{
    internal class MkFPRTN : IFunc<Context, FPRMNum>
    {
        public FPRMNum Run(Context ctx) => ctx.MkFPRTN();
    }
}
