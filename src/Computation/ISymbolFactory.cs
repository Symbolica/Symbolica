using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    public interface ISymbolFactory
    {
        IFunc<Context, BitVecExpr> Create(Bits size, string? name);
    }
}
