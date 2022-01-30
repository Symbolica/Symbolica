using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class SignExtend : BitVector
{
    private readonly IValue _value;

    public SignExtend(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkSignExt((uint) (Size - _value.Size), _value.AsBitVector(context));
    }
}
