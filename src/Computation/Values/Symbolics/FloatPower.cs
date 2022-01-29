using Microsoft.Z3;
using Symbolica.Computation.Exceptions;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class FloatPower : Float, IRealValue
{
    private readonly IValue _left;
    private readonly IValue _right;

    public FloatPower(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override FPExpr AsFloat(Context context)
    {
        throw new UnsupportedSymbolicArithmeticException();
    }

    public RealExpr AsReal(Context context)
    {
        var left = _left is IRealValue l
            ? l.AsReal(context)
            : context.MkFPToReal(_left.AsFloat(context));

        var right = _right is IRealValue r
            ? r.AsReal(context)
            : context.MkFPToReal(_right.AsFloat(context));

        return (RealExpr) context.MkPower(left, right);
    }
}
