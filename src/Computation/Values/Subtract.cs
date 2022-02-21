using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record Subtract : BitVector
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

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVSub(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as Subtract);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (_, IConstantValue r) when r.AsUnsigned().IsZero => left,
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().Subtract(r.AsUnsigned()),
            _ when left.Equals(right) => ConstantUnsigned.CreateZero(left.Size),
            (AggregateOffset l, _) => l.Subtract(right),
            _ => new Subtract(left, right)
        };
    }
}
