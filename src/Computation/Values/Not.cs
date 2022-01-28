using System.Collections.Generic;
using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class Not : Integer
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

    public override BoolExpr AsBool(IContext context)
    {
        return _value is Bool
            ? context.CreateExpr(c => c.MkNot(_value.AsBool(context)))
            : AsBitVector(context).AsBool(context);
    }

    public static IValue Create(IValue value)
    {
        return value is IConstantValue v
            ? v.AsUnsigned().Not()
            : new Not(value);
    }
}
