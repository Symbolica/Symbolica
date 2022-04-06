using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal interface IBlockFactory
{
    IPersistentBlock Create(ISpace space, Section section, IExpression<IType> address, Bits size);
    IPersistentBlock CreateInvalid();
}
