using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class SymbolFactory : ISymbolFactory
{
    public BitVecExpr Create(IContext context, Bits size, string name)
    {
        return context.Execute(c => c.MkBVConst(name, (uint) size));
    }
}
