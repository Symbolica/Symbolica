using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class Select : IValue
{
    private readonly IValue _falseValue;
    private readonly IValue _predicate;
    private readonly IValue _trueValue;

    private Select(IValue predicate, IValue trueValue, IValue falseValue)
    {
        _predicate = predicate;
        _trueValue = trueValue;
        _falseValue = falseValue;
    }

    public Bits Size => _trueValue.Size;

    public IEnumerable<IValue> Children => new[] { _predicate, _trueValue, _falseValue };

    public string? PrintedValue => null;

    public BitVecExpr AsBitVector(IContext context)
    {
        using var t1 = _predicate.AsBool(context);
        using var t2 = _trueValue.AsBitVector(context);
        using var t3 = _falseValue.AsBitVector(context);
        return context.CreateExpr(c => (BitVecExpr) c.MkITE(t1, t2, t3));
    }

    public BoolExpr AsBool(IContext context)
    {
        using var t1 = _predicate.AsBool(context);
        using var t2 = _trueValue.AsBool(context);
        using var t3 = _falseValue.AsBool(context);
        return context.CreateExpr(c => (BoolExpr) c.MkITE(t1, t2, t3));
    }

    public FPExpr AsFloat(IContext context)
    {
        using var t1 = _predicate.AsBool(context);
        using var t2 = _trueValue.AsFloat(context);
        using var t3 = _falseValue.AsFloat(context);
        return context.CreateExpr(c => (FPExpr) c.MkITE(t1, t2, t3));
    }

    public IValue BitCast(Bits targetSize) => this;

    public static IValue Create(IValue predicate, IValue trueValue, IValue falseValue)
    {
        return (predicate, trueValue, falseValue) switch
        {
            (IConstantValue p, _, _) => p.AsBool() ? trueValue : falseValue,
            (_, IConstantValue t, IConstantValue f) when t.AsUnsigned().Equal(f.AsUnsigned()) => t,
            _ => new Select(predicate, trueValue, falseValue)
        };
    }
}
