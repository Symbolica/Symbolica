using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IMemory
{
    IExpression Allocate(Size size);
    IExpression Move(IExpression address, Size size);
    void Free(IExpression address);
    void Write(IExpression address, IExpression value);
    IExpression Read(IExpression address, Size size);
}
