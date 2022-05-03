using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal sealed class SymbolicMemory : IPersistentMemory
{
    private readonly Bytes _alignment;
    private readonly ICollectionFactory _collectionFactory;
    private readonly IBlockFactory _blockFactory;
    private readonly IPersistentList<IPersistentBlock> _blocks;

    private SymbolicMemory(Bytes alignment, ICollectionFactory collectionFactory, IBlockFactory blockFactory,
        IPersistentList<IPersistentBlock> blocks)
    {
        _alignment = alignment;
        _collectionFactory = collectionFactory;
        _blockFactory = blockFactory;
        _blocks = blocks;
    }

    public (IExpression, IPersistentMemory) Allocate(ISpace space, Section section, Bits size)
    {
        var address = CreateAddress(space, size.ToBytes());
        var block = _blockFactory.Create(space, section, address, size);

        return (address, new SymbolicMemory(_alignment, _collectionFactory, _blockFactory,
            _blocks.Add(block)));
    }

    public (IExpression, IPersistentMemory) Move(ISpace space, Section section, IExpression address, Bits size)
    {
        foreach (var (block, index) in _blocks.Select((b, i) => (b, i)))
            if (block.CanFree(space, section, address))
            {
                var newAddress = CreateAddress(space, size.ToBytes());
                var newBlock = block.Move(newAddress, size);

                return (newAddress, new SymbolicMemory(_alignment, _collectionFactory, _blockFactory,
                    _blocks.SetItem(index, newBlock)));
            }

        throw new StateException(StateError.InvalidMemoryMove, space);
    }

    public IPersistentMemory Free(ISpace space, Section section, IExpression address)
    {
        foreach (var (block, index) in _blocks.Select((b, i) => (b, i)))
            if (block.CanFree(space, section, address))
                return new SymbolicMemory(_alignment, _collectionFactory, _blockFactory,
                    _blocks.SetItem(index, _blockFactory.CreateInvalid()));

        throw new StateException(StateError.InvalidMemoryFree, space);
    }

    public IPersistentMemory Write(ISpace space, IExpression address, IExpression value)
    {
        var newBlocks = new List<KeyValuePair<int, IPersistentBlock>>();

        foreach (var (block, index) in _blocks.Select((b, i) => (b, i)))
        {
            var result = block.TryWrite(_collectionFactory, space, Address.Create(address), value);

            if (!result.CanBeSuccess)
                continue;

            newBlocks.Add(KeyValuePair.Create(index, result.Value));

            if (!result.CanBeFailure)
                return new SymbolicMemory(_alignment, _collectionFactory, _blockFactory,
                    _blocks.SetItems(newBlocks));

            space = result.FailureSpace;
        }

        throw new StateException(StateError.InvalidMemoryWrite, space);
    }

    public IExpression Read(ISpace space, IExpression address, Bits size)
    {
        var expression = space.CreateZero(size);

        foreach (var block in _blocks)
        {
            var result = block.TryRead(space, Address.Create(address), size);

            if (!result.CanBeSuccess)
                continue;

            expression = expression.Or(result.Value);

            if (!result.CanBeFailure)
                return expression;

            space = result.FailureSpace;
        }

        throw new StateException(StateError.InvalidMemoryRead, space);
    }

    private IExpression CreateAddress(ISpace space, Bytes size)
    {
        return space.CreateSymbolic(space.PointerSize, null,
            _blocks
                .Where(b => b.IsValid)
                .Select(b => new Func<IExpression, IExpression>(a => IsFullyOutside(space, b, a, size)))
                .Append(a => a
                    .NotEqual(space.CreateZero(a.Size)))
                .Append(a => a
                    .UnsignedLessOrEqual(GetBound(space, a, size)))
                .Append(a => a
                    .UnsignedRemainder(space.CreateConstant(a.Size, (uint) _alignment))
                    .Equal(space.CreateZero(a.Size))));
    }

    private static IExpression IsFullyOutside(ISpace space, IPersistentBlock block, IExpression address, Bytes size)
    {
        return GetBound(space, address, size).UnsignedLessOrEqual(block.Offset)
            .Or(address.UnsignedGreaterOrEqual(GetBound(space, block.Offset, block.Size)));
    }

    private static IExpression GetBound(ISpace space, IExpression address, Bytes size)
    {
        return address.Add(space.CreateConstant(address.Size, (uint) size));
    }

    public static IPersistentMemory Create(Bytes alignment,
        IBlockFactory blockFactory, ICollectionFactory collectionFactory)
    {
        return new SymbolicMemory(alignment, collectionFactory, blockFactory,
            collectionFactory.CreatePersistentList<IPersistentBlock>());
    }
}
