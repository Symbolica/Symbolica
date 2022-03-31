using System.Numerics;

namespace Symbolica.Expression;

public interface ISpace
{
    Bits PointerSize { get; }

    Example GetExample();
    IExpression CreateConstantFloat(Bits size, string value);
    IExpression CreateGarbage(Bits size);
    IProposition CreateProposition(IExpression expression);
    BigInteger GetExampleValue(IExpression expression);
    BigInteger GetSingleValue(IExpression expression);
}
