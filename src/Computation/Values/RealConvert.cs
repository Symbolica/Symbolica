using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record RealConvert : Float, IRealValue
{
    private readonly IRealValue _value;

    public RealConvert(Bits size, IRealValue value)
        : base(size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(ISolver solver)
    {
        throw new UnsupportedSymbolicArithmeticException();
    }

    public RealExpr AsReal(ISolver solver)
    {
        return _value.AsReal(solver);
    }
}
