using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class UnsignedGreaterOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private UnsignedGreaterOrEqual(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(IContext context)
    {
        return context.CreateExpr(c => c.MkBVUGE(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().GreaterOrEqual(r.AsUnsigned()),
            (Address<Bytes> l, _) => Create(l.Aggregate(), right),
            (Address<Bits> l, _) => Create(l.Aggregate(), right),
            (_, Address<Bytes> r) => Create(left, r.Aggregate()),
            (_, Address<Bits> r) => Create(left, r.Aggregate()),
            _ => new UnsignedGreaterOrEqual(left, right)
        };
    }
}
