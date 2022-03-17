using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record LogicalXor : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private LogicalXor(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var left = _left.AsBool(context);
            using var right = _right.AsBool(context);
            return c.MkXor(left, right);
        });
    }

    private static IValue ShortCircuit(IValue left, ConstantBool right)
    {
        return right
            ? LogicalNot.Create(left)
            : LogicalNot.Create(LogicalNot.Create(left));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return right is IConstantValue r
            ? ShortCircuit(left, r.AsBool())
            : left is IConstantValue l
                ? ShortCircuit(right, l.AsBool())
                : new LogicalXor(left, right);
    }
}
