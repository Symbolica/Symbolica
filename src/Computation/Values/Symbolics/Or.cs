using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class Or : Integer
{
    private readonly IValue _left;
    private readonly IValue _right;

    public Or(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkBVOR(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkOr(_left.AsBool(context), _right.AsBool(context));
    }
}
