using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Abstraction.Memory;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal interface IPersistentBlock
{
    bool IsValid { get; }
    IExpression Offset { get; }
    Bytes Size { get; }
    Section Section { get; }
    IExpression Data { get; }

    IPersistentBlock Move(IExpression address, Bits size);
    bool CanFree(ISpace space, Section section, IExpression address);
    Result<IPersistentBlock> TryWrite(ISpace space, IAddress address, IExpression value);
    Result<IExpression> TryRead(ISpace space, IAddress address, Bits size);
    (HashSet<(IExpression, IExpression)> subs, bool) IsDataEquivalentTo(IPersistentBlock other);
    object ToJson();
}
