using System.Numerics;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IStruct
{
    IExpression<IType> Expression { get; }

    IExpression<IType> Read(ISpace space, int index);
    IStruct Write(ISpace space, int index, IExpression<IType> value);
    IStruct Write(ISpace space, int index, BigInteger value);
}
