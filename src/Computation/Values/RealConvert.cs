using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class RealConvert : Float, IRealValue
{
    private readonly IRealValue _value;

    public RealConvert(Bits size, IRealValue value)
        : base(size)
    {
        _value = value;
    }

    public override IEnumerable<IValue> Children => new[] { _value };

    public override string? PrintedValue => null;

    public override FPExpr AsFloat(IContext context)
    {
        throw new UnsupportedSymbolicArithmeticException();
    }

    public RealExpr AsReal(IContext context)
    {
        return _value.AsReal(context);
    }
}
