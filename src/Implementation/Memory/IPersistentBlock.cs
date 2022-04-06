using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal interface IPersistentBlock
{
    bool IsValid { get; }
    IExpression<IType> Address { get; }
    Bytes Size { get; }

    IPersistentBlock Move(IExpression<IType> address, Bits size);
    bool CanFree(ISpace space, Section section, IExpression<IType> address);
    Result<IPersistentBlock> TryWrite(ISpace space, IExpression<IType> address, IExpression<IType> value);
    Result<IExpression<IType>> TryRead(ISpace space, IExpression<IType> address, Bits size);
}
