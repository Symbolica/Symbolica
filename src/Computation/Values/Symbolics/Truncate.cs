using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class Truncate : BitVector
{
    private readonly IValue _value;

    private Truncate(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkExtract((uint) (Size - Bits.One), 0U, _value.AsBitVector(context));
    }

    public static IValue Create(Bits size, IValue value)
    {
        return Value.Unary(value,
            v => v.AsUnsigned().Truncate(size),
            v => new Truncate(size, v));
    }
}
