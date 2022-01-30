using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class FloatAdd : Float
{
    private readonly IValue _left;
    private readonly IValue _right;

    public FloatAdd(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override FPExpr AsFloat(Context context)
    {
        return context.MkFPAdd(context.MkFPRNE(), _left.AsFloat(context), _right.AsFloat(context));
    }
}
