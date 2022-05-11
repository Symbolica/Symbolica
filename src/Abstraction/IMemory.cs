using Symbolica.Abstraction.Memory;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IMemory
{
    IExpression Allocate(Bits size);
    IExpression Allocate(Section section, Bits size);
    IExpression Move(IExpression address, Bits size);
    void Free(IExpression address);
    void Free(Section section, IExpression address);
    void Write(IExpression address, IExpression value);
    IExpression Read(IExpression address, Bits size);
}
