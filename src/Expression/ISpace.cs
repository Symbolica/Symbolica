using System.Numerics;

namespace Symbolica.Expression;

public interface ISpace
{
    Bits PointerSize { get; }

    Example GetExample();
    IExpression<IType> CreateGarbage(Bits size);
    IProposition CreateProposition(IExpression<IType> expression);
    BigInteger GetExampleValue(IExpression<IType> expression);
    BigInteger GetSingleValue(IExpression<IType> expression);
    IExpression<IType> Read(IExpression<IType> buffer, IExpression<IType> offset, Bits size);
    IExpression<IType> Write(IExpression<IType> buffer, IExpression<IType> offset, IExpression<IType> value);
}
