using Symbolica.Abstraction.Memory;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal interface IPersistentMemory : IEquivalent<ExpressionSubs, IPersistentMemory>, IMergeable<IPersistentMemory>
{
    (IExpression, IPersistentMemory) Allocate(Section section, Bits size);
    (IExpression, IPersistentMemory) Move(ISpace space, Section section, IExpression address, Bits size);
    IPersistentMemory Free(ISpace space, Section section, IExpression address);
    IPersistentMemory Write(ISpace space, IExpression address, IExpression value);
    IExpression Read(ISpace space, IExpression address, Bits size);
}
