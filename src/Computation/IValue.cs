using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    public interface IValue
    {
        Bits Size { get; }

        BitVecExpr AsBitVector(Context context);
        BoolExpr AsBool(Context context);
        FPExpr AsFloat(Context context);
    }
}
