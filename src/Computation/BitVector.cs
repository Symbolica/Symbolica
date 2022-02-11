using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract class BitVector : Integer
{
    protected BitVector(Bits size)
        : base(size)
    {
    }

    public sealed override BoolExpr AsBool(IContext context)
    {
        return AsBitVector(context).AsBool(context);
    }
}
