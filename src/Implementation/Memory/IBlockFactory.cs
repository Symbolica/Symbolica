using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal interface IBlockFactory
{
    IPersistentBlock Create(ISpace space, Section section, IExpression address, Size size);
    IPersistentBlock CreateInvalid();
}
