using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class Truncate : BitVector
{
    private readonly IValue _value;

    private Truncate(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkExtract((uint) (Size - Bits.One), 0U, _value.AsBitVector(context)));
    }

    public static IValue Create(Bits size, IValue value)
    {
        return value is IConstantValue v
            ? v.AsUnsigned().Truncate(size)
            : new Truncate(size, value);
    }
}
