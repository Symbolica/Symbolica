using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal interface IPersistentMemory
{
    (IExpression<IType>, IPersistentMemory) Allocate(ISpace space, Section section, Bits size);
    (IExpression<IType>, IPersistentMemory) Move(ISpace space, Section section, IExpression<IType> address, Bits size);
    IPersistentMemory Free(ISpace space, Section section, IExpression<IType> address);
    IPersistentMemory Write(ISpace space, IExpression<IType> address, IExpression<IType> value);
    IExpression<IType> Read(ISpace space, IExpression<IType> address, Bits size);
}
