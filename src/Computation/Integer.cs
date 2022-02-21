using Microsoft.Z3;
using Symbolica.Computation.Values;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract class Integer : IValue
{
    protected Integer(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }

    public abstract BitVecExpr AsBitVector(IContext context);
    public abstract BoolExpr AsBool(IContext context);

    public FPExpr AsFloat(IContext context)
    {
        return context.CreateExpr(c => c.MkFPToFP(AsBitVector(context), Size.GetSort(context)));
    }

    public virtual IValue BitCast(Bits targetSize) => this;

    public virtual IValue ToBits() => Multiply.Create(this, ConstantUnsigned.Create(Size, (uint) Bytes.One.ToBits()));

    public virtual IValue TryMakeConstant(IAssertions assertions)
    {
        var constant = assertions.GetValue(this);
        using var proposition = assertions.GetProposition(Equal.Create(constant, this));
        if (proposition.CanBeFalse)
            return this;

        return constant;
    }
}
