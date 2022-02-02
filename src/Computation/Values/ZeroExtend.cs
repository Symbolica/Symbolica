using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class ZeroExtend : BitVector
{
    private readonly IValue _value;

    private ZeroExtend(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkZeroExt((uint) (Size - _value.Size), _value.AsBitVector(context));
    }

    public static IValue Create(Bits size, IValue value)
    {
        return Value.Create(value,
            v => v.AsUnsigned().Extend(size),
            v => new ZeroExtend(size, v));
    }
}
