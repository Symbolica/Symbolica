using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record Multiply : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Multiply(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVMul(left, right);
    }

    private static IValue ShortCircuit(IValue left, ConstantUnsigned right)
    {
        return right.IsZero
            ? right
            : right.IsOne
                ? left
                : Create(left, right);
    }

    private static IValue Create(IValue left, ConstantUnsigned right)
    {
        return left switch
        {
            IConstantValue l => l.AsUnsigned().Multiply(right),
            Multiply l => Create(l._left, Create(l._right, right)),
            _ => new Multiply(left, right)
        };
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, _) => ShortCircuit(right, l.AsUnsigned()),
            (_, IConstantValue r) => ShortCircuit(left, r.AsUnsigned()),
            _ => new Multiply(left, right)
        };
    }
}
