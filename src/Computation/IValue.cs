using Microsoft.Z3;
using Symbolica.Computation.Values;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal interface IValue
{
    Bits Size { get; }

    BitVecExpr AsBitVector(IContext context);
    BoolExpr AsBool(IContext context);
    FPExpr AsFloat(IContext context);
    IValue BitCast(Bits targetSize);
    IValue ToBits() => Multiply.Create(this, ConstantUnsigned.Create(Size, (uint) Bytes.One.ToBits()));
    IValue TryMakeConstant(IAssertions assertions)
    {
        var constant = assertions.GetValue(this);
        using var proposition = assertions.GetProposition(Equal.Create(constant, this));
        if (proposition.CanBeFalse)
            return this;

        return constant;
    }
}
