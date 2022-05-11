using Symbolica.Abstraction.Memory;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IMemory
{
    IExpression Allocate(Bits size);
    IExpression Allocate(Section section, Bits size);
    IExpression Move(ISpace space, IExpression address, Bits size);
    void Free(ISpace space, IExpression address);
    void Free(ISpace space, Section section, IExpression address);
    void Write(ISpace space, IExpression address, IExpression value);
    IExpression Read(ISpace space, IExpression address, Bits size);
}
