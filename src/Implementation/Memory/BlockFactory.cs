using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Abstraction.Memory;
using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;

namespace Symbolica.Implementation.Memory;

internal sealed class BlockFactory : IBlockFactory
{
    private readonly IExpressionFactory _exprFactory;

    public BlockFactory(IExpressionFactory exprFactory)
    {
        _exprFactory = exprFactory;
    }

    public IPersistentBlock Create(Section section, IExpression address, Bits size)
    {
        return new PersistentBlock(_exprFactory, section, address, _exprFactory.CreateGarbage(size));
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
        public Section Section => throw new ImplementationException("Invalid block has no section.");
        public IExpression Data => throw new ImplementationException("Invalid block has no data.");

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

        public (HashSet<(IExpression, IExpression)> subs, bool) IsDataEquivalentTo(IPersistentBlock other)
        {
            throw new ImplementationException("Invalid block has no data.");
        }

        public object ToJson()
        {
            return GetType().Name;
        }
    }
}
