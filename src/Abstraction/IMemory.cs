using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    public interface IMemory
    {
        IExpression Allocate(Bits size);
        IExpression Move(IExpression address, Bits size);
        void Free(IExpression address);
        void Write(IExpression address, IExpression value);
        IExpression Read(IExpression address, Bits size);
    }
}
