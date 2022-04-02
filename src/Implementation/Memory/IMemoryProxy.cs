using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal interface IMemoryProxy : IMemory
{
    IMemoryProxy Clone(ISpace space);
    IExpression<IType> Allocate(Section section, Bits size);
    void Free(Section section, IExpression<IType> address);
}
