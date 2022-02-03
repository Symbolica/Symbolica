using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class Not : Integer
{
    private readonly IValue _value;

    private Not(IValue value)
        : base(value.Size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkBVNot(_value.AsBitVector(context));
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkNot(_value.AsBool(context));
    }

    public static IValue Create(IValue value)
    {
        return value is IConstantValue v
            ? v.AsUnsigned().Not()
            : new Not(value);
    }
}
