using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class ShiftLeft : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private ShiftLeft(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkBVSHL(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Value.Binary(left, right,
            (l, r) => l.AsUnsigned().ShiftLeft(r.AsUnsigned()),
            (l, r) => new ShiftLeft(l, r));
    }
}
