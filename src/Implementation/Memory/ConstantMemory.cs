using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Abstraction.Memory;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal sealed class ConstantMemory : IPersistentMemory
{
    private readonly Bytes _alignment;
    private readonly IPersistentList<Allocation> _allocations;
    private readonly IBlockFactory _blockFactory;
    private readonly ICollectionFactory _collectionFactory;
    private readonly IExpressionFactory _exprFactory;
    private readonly Bytes _nextAddress;
    private readonly Lazy<int> _equivalencyHash;
    private readonly Lazy<int> _mergeHash;

    private ConstantMemory(Bytes alignment, IBlockFactory blockFactory, ICollectionFactory collectionFactory,
        IExpressionFactory exprFactory, Bytes nextAddress, IPersistentList<Allocation> allocations)
    {
        _alignment = alignment;
        _blockFactory = blockFactory;
        _collectionFactory = collectionFactory;
        _exprFactory = exprFactory;
        _nextAddress = nextAddress;
        _allocations = allocations;
        _equivalencyHash = new(() => EquivalencyHash(false));
        _mergeHash = new(() => EquivalencyHash(true));

        int EquivalencyHash(bool includeSubs)
        {
            var allocationsHash = new HashCode();
            foreach (var allocation in ValidAllocations)
                allocationsHash.Add(allocation.GetEquivalencyHash(includeSubs));

            return HashCode.Combine(_alignment, allocationsHash.ToHashCode());
        }
    }

    private IEnumerable<Allocation> ValidAllocations => _allocations.Where(a => a.Block.IsValid);

    public (IExpression, IPersistentMemory) Allocate(Section section, Bits size)
    {
        var address = CreateAddress();
        var block = _blockFactory.Create(section, address, size);
        var allocation = new Allocation(_nextAddress, block);

        return (address, new ConstantMemory(_alignment, _blockFactory, _collectionFactory,
            _exprFactory, GetNextAddress(size), _allocations.Add(allocation)));
    }

    public (IExpression, IPersistentMemory) Move(ISpace space, Section section, IExpression address, Bits size)
    {
        var (index, allocation) = Allocation.Get(space, address, _allocations);

        if (!allocation.Block.CanFree(space, section, address))
            throw new StateException(StateError.InvalidMemoryMove, space);

        var freedAllocation = new Allocation(allocation.Address, _blockFactory.CreateInvalid());

        var newAddress = CreateAddress();
        var newBlock = allocation.Block.Move(newAddress, size);
        var newAllocation = new Allocation(_nextAddress, newBlock);

        return (newAddress, new ConstantMemory(_alignment, _blockFactory, _collectionFactory,
            _exprFactory, GetNextAddress(size), _allocations.SetItem(index, freedAllocation).Add(newAllocation)));
    }

    public IPersistentMemory Free(ISpace space, Section section, IExpression address)
    {
        var (index, allocation) = Allocation.Get(space, address, _allocations);

        if (!allocation.Block.CanFree(space, section, address))
            throw new StateException(StateError.InvalidMemoryFree, space);

        var allocations = _allocations.SetItem(index, new Allocation(allocation.Address, _blockFactory.CreateInvalid()));
        var invalidTail = allocations.Reverse().TakeWhile(a => !a.Block.IsValid).Reverse();

        var (prunedAllocations, nextAddress) = invalidTail.Any()
            ? (_collectionFactory.CreatePersistentList<Allocation>().AddRange(allocations.SkipLast(invalidTail.Count())), invalidTail.First().Address)
            : (allocations, _nextAddress);

        return new ConstantMemory(_alignment, _blockFactory, _collectionFactory, _exprFactory, nextAddress, prunedAllocations);
    }

    public IPersistentMemory Write(ISpace space, IExpression address, IExpression value)
    {
        // This is the ptr to wthru buffer and it seems to cycle, but not exactly
        // if (!address.IsSymbolic && address.GetSingleValue(space) == (736 + 0 + 64 + 200 + 13) && !value.IsSymbolic && value.GetSingleValue(space) != 0)
        //     Debugger.Break();

        // This seems to be vlSelf->iob_cache__DOT__cache_memory__DOT__genblk1__DOT__write_throught_buffer__DOT__fifo_ocupancy
        // = __Vdly__iob_cache__DOT__cache_memory__DOT__genblk1__DOT__write_throught_buffer__DOT__fifo_ocupancy;
        // in Viob_cache___024root___sequent__TOP__2
        // if (!address.IsSymbolic && address.GetSingleValue(space) == (736 + 0 + 64 + 200 + 280) && !value.IsSymbolic && value.GetSingleValue(space) != 0)
        //     Debugger.Break();

        // In SEQUENT_TOP3 there is some logic that sets a register based on wstrb being even or odd
        // if (address.GetSingleValue(space) == (736 + 0 + 64 + 1432 + 640 + 0 + 0))
        //     Debugger.Break();

        var newAllocations = new List<KeyValuePair<int, Allocation>>();

        while (true)
        {
            var (index, allocation) = Allocation.Get(space, address, _allocations);

            var block = AggregateBlock.TryCreate(_collectionFactory, _exprFactory, space, address, allocation);

            var result = block.TryWrite(space, Address.Create(_exprFactory, address), value);

            if (!result.CanBeSuccess)
                throw new StateException(StateError.InvalidMemoryWrite, space);

            newAllocations.Add(KeyValuePair.Create(index, new Allocation(allocation.Address, result.Value)));

            if (!result.CanBeFailure)
                return new ConstantMemory(_alignment, _blockFactory, _collectionFactory,
                    _exprFactory, _nextAddress, _allocations.SetItems(newAllocations));

            space = result.FailureSpace;
        }
    }

    public IExpression Read(ISpace space, IExpression address, Bits size)
    {
        var expression = _exprFactory.CreateZero(size);

        while (true)
        {
            var (_, allocation) = Allocation.Get(space, address, _allocations);
            var result = allocation.Block.TryRead(space, Address.Create(_exprFactory, address), size);

            if (!result.CanBeSuccess)
                throw new StateException(StateError.InvalidMemoryRead, space);

            expression = expression.Or(result.Value);

            if (!result.CanBeFailure)
                return expression;

            space = result.FailureSpace;
        }
    }

    private IExpression CreateAddress()
    {
        return _exprFactory.CreateConstant(_exprFactory.PointerSize, (uint) _nextAddress);
    }

    private Bytes GetNextAddress(Bits size)
    {
        return size == Bits.Zero
            ? _nextAddress + _alignment
            : (_nextAddress + size.ToBytes()).AlignTo(_alignment);
    }

    public static IPersistentMemory Create(Bytes alignment,
        IBlockFactory blockFactory, ICollectionFactory collectionFactory, IExpressionFactory exprFactory)
    {
        var nullAllocation = new Allocation(Bytes.Zero, blockFactory.CreateInvalid());

        return new ConstantMemory(alignment, blockFactory, collectionFactory,
            exprFactory, alignment, collectionFactory.CreatePersistentList<Allocation>().Add(nullAllocation));
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IPersistentMemory other)
    {
        return other is ConstantMemory cm
            ? ValidAllocations.IsSequenceEquivalentTo<ExpressionSubs, Allocation>(cm.ValidAllocations)
                .And((new(), _alignment == cm._alignment))
            : (new(), false);
    }

    public object ToJson()
    {
        return new
        {
            Alignment = (uint) _alignment,
            Allocations = _allocations
                .Where(a => a.Block.IsValid)
                .Select(a => a.ToJson())
                .ToArray()
        };
    }

    public int GetEquivalencyHash(bool includeSubs)
    {
        return includeSubs
            ? _mergeHash.Value
            : _equivalencyHash.Value;
    }
}
