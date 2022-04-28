using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;

namespace Symbolica.Implementation.Memory;

internal sealed class BlockFactory : IBlockFactory
{
    public IPersistentBlock Create(ISpace space, Section section, IExpression address, Bits size)
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
        public IExpression Offset => throw new ImplementationException("Invalid block has no address.");
        public Bytes Size => throw new ImplementationException("Invalid block has no size.");

        public IPersistentBlock Move(IExpression address, Bits size)
        {
            return this;
        }

        public bool CanFree(ISpace space, Section section, IExpression address)
        {
            return false;
        }

        public Result<IPersistentBlock> TryWrite(ISpace space, IAddress address, IExpression value)
        {
            return Result<IPersistentBlock>.Failure(space);
        }

        public Result<IExpression> TryRead(ISpace space, IAddress address, Bits size)
        {
            return Result<IExpression>.Failure(space);
        }

        public IExpression Data(ISpace space)
        {
            throw new global::System.NotImplementedException();
        }
    }
}
