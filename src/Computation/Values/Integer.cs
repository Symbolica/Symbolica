using Microsoft.Z3;
using Symbolica.Computation.Values.Symbolics;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal abstract class Integer : Value
{
    protected Integer(Bits size)
    {
        Size = size;
    }

    public override Bits Size { get; }

    public override FPExpr AsFloat(Context context)
    {
        return context.MkFPToFP(AsBitVector(context), Size.GetSort(context));
    }

    public static IValue And(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().And(r.AsUnsigned()),
            (l, r) => new And(l, r));
    }

    public static IValue Or(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().Or(r.AsUnsigned()),
            (l, r) => new Or(l, r));
    }

    protected static IValue Not(IValue value)
    {
        return Unary(value,
            v => v.AsUnsigned().Not(),
            v => new Not(v));
    }

    public static IValue Xor(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().Xor(r.AsUnsigned()),
            (l, r) => new Xor(l, r));
    }
}
