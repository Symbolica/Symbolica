using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IValue
{
    Bits Size { get; }

    BitVecExpr AsBitVector(IContext context);
    BoolExpr AsBool(IContext context);
    FPExpr AsFloat(IContext context);
}
