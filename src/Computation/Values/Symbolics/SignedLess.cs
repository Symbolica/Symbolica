using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class SignedLess : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedLess(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkBVSLT(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Value.Create(left, right,
            (l, r) => l.AsSigned().Less(r.AsSigned()),
            (l, r) => new SignedLess(l, r));
    }
}
