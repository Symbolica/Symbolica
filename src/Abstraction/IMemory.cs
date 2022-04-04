using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Abstraction;

public interface IMemory
{
    Address Allocate(Bits size);
    Address Move(Address address, Bits size);
    void Free(Address address);
    void Write(Address address, IExpression<IType> value);
    IExpression<IType> Read(Address address, Bits size);
}
