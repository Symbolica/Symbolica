using System.Collections.Generic;
using Microsoft.Z3;

namespace Symbolica.Computation;

internal interface IRealValue : IValue
{
    RealExpr AsReal(ISolver solver);
}
