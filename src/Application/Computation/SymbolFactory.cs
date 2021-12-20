using Microsoft.Z3;
using Symbolica.Computation;
using Symbolica.Expression;

namespace Symbolica.Application.Computation
{
    internal sealed class SymbolFactory : ISymbolFactory
    {
        public BitVecExpr Create(Context context, Bits size, string name)
        {
            return context.MkBVConst(name, (uint) size);
        }
    }
}
