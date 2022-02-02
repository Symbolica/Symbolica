using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics;

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

    public BitVecExpr AsBitVector(Context context)
    {
        return (BitVecExpr) context.MkITE(_predicate.AsBool(context),
            _trueValue.AsBitVector(context),
            _falseValue.AsBitVector(context));
    }

    public BoolExpr AsBool(Context context)
    {
        return (BoolExpr) context.MkITE(_predicate.AsBool(context),
            _trueValue.AsBool(context),
            _falseValue.AsBool(context));
    }

    public FPExpr AsFloat(Context context)
    {
        return (FPExpr) context.MkITE(_predicate.AsBool(context),
            _trueValue.AsFloat(context),
            _falseValue.AsFloat(context));
    }

    public static IValue Create(IValue predicate, IValue trueValue, IValue falseValue)
    {
        return Value.Create(predicate, trueValue, falseValue,
            (p, t, f) => p.AsBool() ? t : f,
            (p, t, f) => new Select(p, t, f));
    }
}
