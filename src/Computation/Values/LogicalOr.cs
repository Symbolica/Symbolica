using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class LogicalOr : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private LogicalOr(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<IValue> Children => new[] { _left, _right };

    public override string? PrintedValue => null;

    public override BoolExpr AsBool(IContext context)
    {
        using var t1 = _left.AsBool(context);
        using var t2 = _right.AsBool(context);
        return context.CreateExpr(c => c.MkOr(t1, t2));
    }

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
