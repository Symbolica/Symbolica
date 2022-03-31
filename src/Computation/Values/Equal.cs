using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record Equal : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Equal(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<IValue> Children => new[] { _left, _right };

    public override string? PrintedValue => null;

    public override BoolExpr AsBool(ISolver solver)
    {
        return _left is Bool || _right is Bool
            ? Logical(solver)
            : Bitwise(solver);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as Equal);
    }

    private BoolExpr Logical(ISolver solver)
    {
        using var left = _left.AsBool(solver);
        using var right = _right.AsBool(solver);
        return solver.Context.MkEq(left, right);
    }

    private BoolExpr Bitwise(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkEq(left, right);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().Equal(r.AsUnsigned()),
            (Address<Bits> l, _) => Create(l.Aggregate(), right),
            (Address<Bytes> l, _) => Create(l.Aggregate(), right),
            (_, Address<Bits> r) => Create(left, r.Aggregate()),
            (_, Address<Bytes> r) => Create(left, r.Aggregate()),
            _ => new Equal(left, right)
        };
    }
}
