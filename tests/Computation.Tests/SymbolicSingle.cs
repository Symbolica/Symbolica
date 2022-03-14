using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class SymbolicSingle : Float
{
    private readonly float _value;

    public SymbolicSingle(float value)
        : base((Bits) 32U)
    {
        _value = value;
    }

    public override IEnumerable<IValue> Children => Enumerable.Empty<IValue>();

    public override string? PrintedValue => _value.ToString();

    public override FPExpr AsFloat(IContext context)
    {
        using var sort = Size.GetSort(context);
        return context.CreateExpr(c => c.MkFP(_value, sort));
    }
}
