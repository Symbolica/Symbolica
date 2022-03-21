using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record LogicalAnd : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private LogicalAnd(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(ISolver solver)
    {
        using var left = _left.AsBool(solver);
        using var right = _right.AsBool(solver);
        return solver.Context.MkAnd(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as LogicalAnd);
    }

    private static IValue ShortCircuit(IValue left, ConstantBool right)
    {
        return right
            ? LogicalNot.Create(LogicalNot.Create(left))
            : right;
    }

    public static IValue Create(IValue left, IValue right)
    {
        return right is IConstantValue r
            ? ShortCircuit(left, r.AsBool())
            : left is IConstantValue l
                ? ShortCircuit(right, l.AsBool())
                : new LogicalAnd(left, right);
    }
}
