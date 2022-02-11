using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

public interface ISymbolFactory
{
    BitVecExpr Create(IContext context, Bits size, string name);
}
