using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class SignedDivide : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedDivide(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkBVSDiv(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Value.Binary(left, right,
            (l, r) => l.AsSigned().Divide(r.AsSigned()),
            (l, r) => new SignedDivide(l, r));
    }
}
