using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record UnsignedGreater : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private UnsignedGreater(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<IValue> Children => new[] { _left, _right };

    public override string? PrintedValue => null;

    public override Microsoft.Z3.BoolExpr AsBool(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVUGT(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as UnsignedGreater);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().Greater(r.AsUnsigned()),
            (Address<Bytes> l, _) => Create(l.Aggregate(), right),
            (Address<Bits> l, _) => Create(l.Aggregate(), right),
            (_, Address<Bytes> r) => Create(left, r.Aggregate()),
            (_, Address<Bits> r) => Create(left, r.Aggregate()),
            _ => new UnsignedGreater(left, right)
        };
    }
}
