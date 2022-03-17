using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record Select : IValue
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

    public BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var predicate = _predicate.AsBool(context);
            using var trueValue = _trueValue.AsBitVector(context);
            using var falseValue = _falseValue.AsBitVector(context);
            return (BitVecExpr) c.MkITE(predicate, trueValue, falseValue);
        });
    }

    public BoolExpr AsBool(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var predicate = _predicate.AsBool(context);
            using var trueValue = _trueValue.AsBool(context);
            using var falseValue = _falseValue.AsBool(context);
            return (BoolExpr) c.MkITE(predicate, trueValue, falseValue);
        });
    }

    public FPExpr AsFloat(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var predicate = _predicate.AsBool(context);
            using var trueValue = _trueValue.AsFloat(context);
            using var falseValue = _falseValue.AsFloat(context);
            return (FPExpr) c.MkITE(predicate, trueValue, falseValue);
        });
    }

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
