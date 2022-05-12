using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal interface IPersistentBlock
{
    bool IsValid { get; }
    IExpression Address { get; }
    IExpression Data { get; }

    IPersistentBlock Move(IExpression address, Bits size);
    bool CanFree(ISpace space, Section section, IExpression address);
    Result TryWrite(ISpace space, IExpression address, IExpression value);
    Result TryRead(ISpace space, IExpression address, Bits size);
}
