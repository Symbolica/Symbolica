using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class Equal : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Equal(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return _left is Bool
            ? context.MkEq(_left.AsBool(context), _right.AsBool(context))
            : context.MkEq(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Value.Binary(left, right,
            (l, r) => l.AsUnsigned().Equal(r.AsUnsigned()),
            (l, r) => new Equal(l, r));
    }
}
