using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Implementation.Memory;

internal interface IPersistentMemory
{
    (Address, IPersistentMemory) Allocate(ISpace space, Section section, Bits size);
    (Address, IPersistentMemory) Move(ISpace space, Section section, Address address, Bits size);
    IPersistentMemory Free(ISpace space, Section section, Address address);
    IPersistentMemory Write(ISpace space, Address address, IExpression<IType> value);
    IExpression<IType> Read(ISpace space, Address address, Bits size);
}
