using System.Collections.Generic;
using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class Not : BitVector
{
    private readonly IValue _value;

    private Not(IValue value)
        : base(value.Size)
    {
        _value = value;
    }

    public override IEnumerable<IValue> Children => new[] { _value };

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkBVNot(_value.AsBitVector(context)));
    }

    public static IValue Create(IValue value)
    {
        return value switch
        {
            IConstantValue v => v.AsUnsigned().Not(),
            Not v => v._value,
            _ => new Not(value)
        };
    }
}
