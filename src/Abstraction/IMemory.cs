using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IMemory
{
    IExpression<IType> Allocate(Bits size);
    IExpression<IType> Move(IExpression<IType> address, Bits size);
    void Free(IExpression<IType> address);
    void Write(IExpression<IType> address, IExpression<IType> value);
    IExpression<IType> Read(IExpression<IType> address, Bits size);
}
