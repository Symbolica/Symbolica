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

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.Execute(c => c.MkSignExt((uint) (Size - _value.Size), _value.AsBitVector(context)));
    }

    public static IValue Create(Bits size, IValue value)
    {
        return value is IConstantValue v
            ? v.AsSigned().Extend(size)
            : new SignExtend(size, value);
    }
}
