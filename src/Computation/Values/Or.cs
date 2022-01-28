using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class Or : Integer
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Or(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<IValue> Children => new[] { _left, _right };

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkBVOR(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    public override BoolExpr AsBool(IContext context)
    {
        return _left is Bool || _right is Bool
            ? context.CreateExpr(c => c.MkOr(_left.AsBool(context), _right.AsBool(context)))
            : AsBitVector(context).AsBool(context);
    }

    private static IValue ShortCircuit(IValue left, ConstantUnsigned right)
    {
        return right.IsZero
            ? left
            : right.Not().IsZero
                ? right
                : Create(left, right);
    }

    private static IValue Create(IValue left, ConstantUnsigned right)
    {
        return left switch
        {
            IConstantValue l => l.AsUnsigned().Or(right),
            Or l => new Or(l._left, Create(l._right, right)),
            _ => new Or(left, right)
        };
    }

    public static IValue Create(IValue left, IValue right)
    {
        return right is IConstantValue r
            ? ShortCircuit(left, r.AsUnsigned())
            : left is IConstantValue l
                ? ShortCircuit(right, l.AsUnsigned())
                : new Or(left, right);
    }
}
