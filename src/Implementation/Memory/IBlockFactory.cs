using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal interface IBlockFactory
{
    IPersistentBlock Create(Section section, IExpression address, Bits size);
    IPersistentBlock CreateInvalid();
}
