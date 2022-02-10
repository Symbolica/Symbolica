using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class SignedLessOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedLessOrEqual(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(IContext context)
    {
        return context.Execute(c => c.MkBVSLE(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsSigned().LessOrEqual(r.AsSigned())
            : new SignedLessOrEqual(left, right);
    }
}
