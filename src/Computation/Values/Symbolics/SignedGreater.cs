using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class SignedGreater : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedGreater(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkBVSGT(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Value.Binary(left, right,
            (l, r) => l.AsSigned().Greater(r.AsSigned()),
            (l, r) => new SignedGreater(l, r));
    }
}
