using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class SignExtend : BitVector
{
    private readonly IValue _value;

    private SignExtend(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkSignExt((uint) (Size - _value.Size), _value.AsBitVector(context));
    }

    public static IValue Create(Bits size, IValue value)
    {
        return Value.Create(value,
            v => v.AsSigned().Extend(size),
            v => new SignExtend(size, v));
    }
}
