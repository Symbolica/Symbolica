using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal interface IPersistentBlock
{
    bool IsValid { get; }
    IExpression Offset { get; }
    Bytes Size { get; }
    Section Section { get; }

    IExpression Data(ISpace space);

    IPersistentBlock Move(IExpression address, Bits size);
    bool CanFree(ISpace space, Section section, IExpression address);
    Result<IPersistentBlock> TryWrite(ICollectionFactory collectionFactory, ISpace space, IAddress address, IExpression value);
    Result<IExpression> TryRead(ISpace space, IAddress address, Bits size);
}
