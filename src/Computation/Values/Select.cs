using System.Numerics;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class Select : IValue
{
    private readonly IValue _falseValue;
    private readonly IValue _predicate;
    private readonly IValue _trueValue;

    public Select(IValue predicate, IValue trueValue, IValue falseValue)
    {
        _predicate = predicate;
        _trueValue = trueValue;
        _falseValue = falseValue;
    }

    public Bits Size => _trueValue.Size;

    public BigInteger AsConstant(IContext context)
    {
        return AsBitVector(context).AsConstant();
    }

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
}
