using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class Xor : Integer
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Xor(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkBVXOR(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkXor(_left.AsBool(context), _right.AsBool(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Value.Create(left, right,
            (l, r) => l.AsUnsigned().Xor(r.AsUnsigned()),
            (l, r) => new Xor(l, r));
    }
}
