using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Implementation.Exceptions;

namespace Symbolica.Implementation.Memory;

internal sealed class BlockFactory : IBlockFactory
{
    public IPersistentBlock Create(ISpace space, Section section, Address address, Bits size)
    {
        return new PersistentBlock(section, address, space.CreateGarbage(size));
    }

    public IPersistentBlock CreateInvalid()
    {
        return InvalidBlock.Instance;
    }

    private sealed class InvalidBlock : IPersistentBlock
    {
        private InvalidBlock()
        {
        }

        public static IPersistentBlock Instance => new InvalidBlock();

        public bool IsValid => false;
        public Address Address => throw new ImplementationException("Invalid block has no address.");
        public Bytes Size => throw new ImplementationException("Invalid block has no size.");

        public IPersistentBlock Move(Address address, Bits size)
        {
            return this;
        }

        public bool CanFree(ISpace space, Section section, Address address)
        {
            return false;
        }

        public Result<IPersistentBlock> TryWrite(ISpace space, Address address, IExpression<IType> value)
        {
            return Result<IPersistentBlock>.Failure(space);
        }

        public Result<IExpression<IType>> TryRead(ISpace space, Address address, Bits size)
        {
            return Result<IExpression<IType>>.Failure(space);
        }
    }
}
