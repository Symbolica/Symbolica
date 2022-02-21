using System.Collections.Generic;
using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class Subtract : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Subtract(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<IValue> Children => new[] { _left, _right };

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkBVSub(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().Subtract(r.AsUnsigned()),
            (_, IConstantValue r) when r.AsUnsigned().IsZero => left,
            (AggregateOffset l, _) => l.Subtract(right),
            _ => new Subtract(left, right)
        };
    }
}
