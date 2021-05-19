using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory
{
    internal interface IMemoryProxy : IMemory
    {
        IMemoryProxy Clone(ISpace space);
        IExpression Allocate(Section section, Bits size);
        void Free(Section section, IExpression address);
    }
}