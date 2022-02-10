using System.Numerics;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

public interface IValue
{
    Bits Size { get; }

    BigInteger AsConstant(IContext context);
    BitVecExpr AsBitVector(IContext context);
    BoolExpr AsBool(IContext context);
    FPExpr AsFloat(IContext context);
}
