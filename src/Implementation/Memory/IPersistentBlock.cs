using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Implementation.Memory;

internal interface IPersistentBlock
{
    bool IsValid { get; }
    Address Address { get; }
    Bytes Size { get; }

    IPersistentBlock Move(Address address, Bits size);
    bool CanFree(ISpace space, Section section, Address address);
    Result<IPersistentBlock> TryWrite(ISpace space, Address address, IExpression<IType> value);
    Result<IExpression<IType>> TryRead(ISpace space, Address address, Bits size);
}
