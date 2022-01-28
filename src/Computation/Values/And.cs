using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class And : Integer
{
    private readonly IValue _left;
    private readonly IValue _right;

    private And(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<IValue> Children => new[] { _left, _right };

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkBVAND(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    public override BoolExpr AsBool(IContext context)
    {
        return _left is Bool || _right is Bool
            ? context.CreateExpr(c => c.MkAnd(_left.AsBool(context), _right.AsBool(context)))
            : AsBitVector(context).AsBool(context);
    }

    private static IValue ShortCircuit(IValue left, ConstantUnsigned right)
    {
        return right.IsZero
            ? right
            : right.Not().IsZero
                ? left
                : Create(left, right);
    }

    private static IValue Create(IValue left, ConstantUnsigned right)
    {
        return left switch
        {
            IConstantValue l => l.AsUnsigned().And(right),
            And l => new And(l._left, Create(l._right, right)),
            _ => new And(left, right)
        };
    }

    public static IValue Create(IValue left, IValue right)
    {
        return right is IConstantValue r
            ? ShortCircuit(left, r.AsUnsigned())
            : left is IConstantValue l
                ? ShortCircuit(right, l.AsUnsigned())
                : new And(left, right);
    }
}
