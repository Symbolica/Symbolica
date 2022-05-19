using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Abstraction.Memory;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal sealed class SymbolicMemory : IPersistentMemory
{
    private readonly Bytes _alignment;
    private readonly ICollectionFactory _collectionFactory;
    private readonly IBlockFactory _blockFactory;
    private readonly IExpressionFactory _exprFactory;
    private readonly IPersistentList<IPersistentBlock> _blocks;

    private SymbolicMemory(Bytes alignment, ICollectionFactory collectionFactory, IBlockFactory blockFactory,
        IExpressionFactory exprFactory, IPersistentList<IPersistentBlock> blocks)
    {
        _alignment = alignment;
        _collectionFactory = collectionFactory;
        _blockFactory = blockFactory;
        _exprFactory = exprFactory;
        _blocks = blocks;
    }

    public (IExpression, IPersistentMemory) Allocate(Section section, Bits size)
    {
        var address = CreateAddress(size.ToBytes());
        var block = _blockFactory.Create(section, address, size);

        return (address, new SymbolicMemory(_alignment, _collectionFactory, _blockFactory,
            _exprFactory, _blocks.Add(block)));
    }

    public (IExpression, IPersistentMemory) Move(ISpace space, Section section, IExpression address, Bits size)
    {
        foreach (var (block, index) in _blocks.Select((b, i) => (b, i)))
            if (block.CanFree(space, section, address))
            {
                var newAddress = CreateAddress(size.ToBytes());
                var newBlock = block.Move(newAddress, size);

                return (newAddress, new SymbolicMemory(_alignment, _collectionFactory, _blockFactory,
                    _exprFactory, _blocks.SetItem(index, newBlock)));
            }

        throw new StateException(StateError.InvalidMemoryMove, space);
    }

    public IPersistentMemory Free(ISpace space, Section section, IExpression address)
    {
        foreach (var (block, index) in _blocks.Select((b, i) => (b, i)))
            if (block.CanFree(space, section, address))
                return new SymbolicMemory(_alignment, _collectionFactory, _blockFactory,
                    _exprFactory, _blocks.SetItem(index, _blockFactory.CreateInvalid()));

        throw new StateException(StateError.InvalidMemoryFree, space);
    }

    public IPersistentMemory Write(ISpace space, IExpression address, IExpression value)
    {
        var newBlocks = new List<KeyValuePair<int, IPersistentBlock>>();

        foreach (var (block, index) in _blocks.Select((b, i) => (b, i)))
        {
            var result = block.TryWrite(space, Address.Create(_exprFactory, address), value);

            if (!result.CanBeSuccess)
                continue;

            newBlocks.Add(KeyValuePair.Create(index, result.Value));

            if (!result.CanBeFailure)
                return new SymbolicMemory(_alignment, _collectionFactory, _blockFactory,
                    _exprFactory, _blocks.SetItems(newBlocks));

            space = result.FailureSpace;
        }

        throw new StateException(StateError.InvalidMemoryWrite, space);
    }

    public IExpression Read(ISpace space, IExpression address, Bits size)
    {
        var expression = _exprFactory.CreateZero(size);

        foreach (var block in _blocks)
        {
            var result = block.TryRead(space, Address.Create(_exprFactory, address), size);

            if (!result.CanBeSuccess)
                continue;

            expression = expression.Or(result.Value);

            if (!result.CanBeFailure)
                return expression;

            space = result.FailureSpace;
        }

        throw new StateException(StateError.InvalidMemoryRead, space);
    }

    private IExpression CreateAddress(Bytes size)
    {
        return _exprFactory.CreateSymbolic(_exprFactory.PointerSize, null,
            _blocks
                .Where(b => b.IsValid)
                .Select(b => new Func<IExpression, IExpression>(a => IsFullyOutside(b, a, size)))
                .Append(a => a.NotEqual(_exprFactory.CreateZero(a.Size)))
                .Append(a => a.UnsignedLessOrEqual(GetBound(a, size)))
                .Append(a => a
                    .UnsignedRemainder(_exprFactory.CreateConstant(a.Size, (uint) _alignment))
                    .Equal(_exprFactory.CreateZero(a.Size))));
    }

    private IExpression IsFullyOutside(IPersistentBlock block, IExpression address, Bytes size)
    {
        return GetBound(address, size).UnsignedLessOrEqual(block.Offset)
            .Or(address.UnsignedGreaterOrEqual(GetBound(block.Offset, block.Size)));
    }

    private IExpression GetBound(IExpression address, Bytes size)
    {
        return address.Add(_exprFactory.CreateConstant(address.Size, (uint) size));
    }

    public static IPersistentMemory Create(Bytes alignment,
        IBlockFactory blockFactory, ICollectionFactory collectionFactory, IExpressionFactory exprFactory)
    {
        return new SymbolicMemory(alignment, collectionFactory, blockFactory,
            exprFactory, collectionFactory.CreatePersistentList<IPersistentBlock>());
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(IPersistentMemory other)
    {
        static IEnumerable<IPersistentBlock> ValidAllocations(SymbolicMemory memory)
        {
            return memory._blocks.Where(a => a.IsValid);
        }

        // TODO: This is relying on the order of allocations in both sides being equal, which could/should be relaxed
        // but that will require being able to lookup blocks by equivalent symbolic address
        return other is SymbolicMemory sm
            ? (new HashSet<(IExpression, IExpression)>(), _alignment == sm._alignment)
                .And(ValidAllocations(this)
                    .IsSequenceEquivalentTo(
                        ValidAllocations(sm),
                        (a, b) => a.Offset
                            .IsEquivalentTo(b.Offset)
                            .And((new(), a.Section == b.Section))
                            .And(a.IsDataEquivalentTo(b))))
            : (new(), false);
    }

    public object ToJson()
    {
        return new
        {
            Alignment = (uint) _alignment,
            Allocations = _blocks.Where(a => a.IsValid).Select(a => a.ToJson()).ToArray()
        };
    }
}
