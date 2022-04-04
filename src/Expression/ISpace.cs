using System.Numerics;
using Symbolica.Expression.Values;

namespace Symbolica.Expression;

public interface ISpace
{
    Bits PointerSize { get; }

    Example GetExample();
    IExpression<IType> CreateGarbage(Bits size);
    IProposition CreateProposition(IExpression<IType> expression);
    BigInteger GetExampleValue(IExpression<IType> expression);
    BigInteger GetSingleValue(IExpression<IType> expression);
    IExpression<IType> Read(IExpression<IType> buffer, Address offset, Bits size);
    bool TryGetSingleValue(IExpression<IType> expression, out BigInteger constant);
    IExpression<IType> Write(IExpression<IType> buffer, Address offset, IExpression<IType> value);
}
