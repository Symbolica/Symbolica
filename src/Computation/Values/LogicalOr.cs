using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record LogicalOr : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private LogicalOr(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(ISolver solver)
    {
        using var left = _left.AsBool(solver);
        using var right = _right.AsBool(solver);
        return solver.Context.MkOr(left, right);
    }

    public override bool Equals(IValue? other) => Equals(other as LogicalOr);

    private static IValue ShortCircuit(IValue left, ConstantBool right)
    {
        return right
            ? right
            : LogicalNot.Create(LogicalNot.Create(left));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return right is IConstantValue r
            ? ShortCircuit(left, r.AsBool())
            : left is IConstantValue l
                ? ShortCircuit(right, l.AsBool())
                : new LogicalOr(left, right);
    }
}
