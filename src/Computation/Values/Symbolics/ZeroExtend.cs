using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class ZeroExtend : BitVector
{
    private readonly IValue _value;

    public ZeroExtend(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkZeroExt((uint) (Size - _value.Size), _value.AsBitVector(context));
    }
}
