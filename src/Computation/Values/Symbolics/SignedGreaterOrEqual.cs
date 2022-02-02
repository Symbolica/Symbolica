using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class SignedGreaterOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedGreaterOrEqual(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkBVSGE(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Value.Binary(left, right,
            (l, r) => l.AsSigned().GreaterOrEqual(r.AsSigned()),
            (l, r) => new SignedGreaterOrEqual(l, r));
    }
}
