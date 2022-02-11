using Microsoft.Z3;

namespace Symbolica.Computation;

internal interface IRealValue : IValue
{
    RealExpr AsReal(IContext context);
}
