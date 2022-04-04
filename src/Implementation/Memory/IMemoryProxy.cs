using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Implementation.Memory;

internal interface IMemoryProxy : IMemory
{
    IMemoryProxy Clone(ISpace space);
    Address Allocate(Section section, Bits size);
    void Free(Section section, Address address);
}
