using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Implementation.Memory;

internal interface IBlockFactory
{
    IPersistentBlock Create(ISpace space, Section section, Address address, Bits size);
    IPersistentBlock CreateInvalid();
}
