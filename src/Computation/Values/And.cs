using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class And : BitVector
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
            And l => Create(l._left, Create(l._right, right)),
            _ => new And(left, right)
        };
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, _) => ShortCircuit(right, l.AsUnsigned()),
            (_, IConstantValue r) => ShortCircuit(left, r.AsUnsigned()),
            _ => new And(left, right)
        };
    }
}
