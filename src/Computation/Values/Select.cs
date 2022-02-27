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
        return context.CreateExpr(c => (BitVecExpr) c.MkITE(_predicate.AsBool(context),
            _trueValue.AsBitVector(context),
            _falseValue.AsBitVector(context)));
    }

    public BoolExpr AsBool(IContext context)
    {
        return context.CreateExpr(c => (BoolExpr) c.MkITE(_predicate.AsBool(context),
            _trueValue.AsBool(context),
            _falseValue.AsBool(context)));
    }

    public FPExpr AsFloat(IContext context)
    {
        return context.CreateExpr(c => (FPExpr) c.MkITE(_predicate.AsBool(context),
            _trueValue.AsFloat(context),
            _falseValue.AsFloat(context)));
    }

    public IValue ToBits() => this;

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
