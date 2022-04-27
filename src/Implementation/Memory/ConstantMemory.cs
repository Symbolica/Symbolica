using System;
using System.Collections.Generic;
using System.Diagnostics;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal sealed class ConstantMemory : IPersistentMemory
{
    private readonly Bytes _alignment;
    private readonly IPersistentList<Allocation> _allocations;
    private readonly IBlockFactory _blockFactory;
    private readonly ICollectionFactory _collectionFactory;
    private readonly Bytes _nextAddress;

    private ConstantMemory(Bytes alignment, IBlockFactory blockFactory, ICollectionFactory collectionFactory,
        Bytes nextAddress, IPersistentList<Allocation> allocations)
    {
        _alignment = alignment;
        _blockFactory = blockFactory;
        _collectionFactory = collectionFactory;
        _nextAddress = nextAddress;
        _allocations = allocations;
    }

    public (IExpression, IPersistentMemory) Allocate(ISpace space, Section section, Bits size)
    {
        var address = CreateAddress(space);
        var block = _blockFactory.Create(space, section, address, size);
        var allocation = new Allocation(_nextAddress, block);

        return (address, new ConstantMemory(_alignment, _blockFactory, _collectionFactory,
            GetNextAddress(size), _allocations.Add(allocation)));
    }

    public (IExpression, IPersistentMemory) Move(ISpace space, Section section, IExpression address, Bits size)
    {
        var (index, allocation) = GetAllocation(space, address);

        if (!allocation.Block.CanFree(space, section, address))
            throw new StateException(StateError.InvalidMemoryMove, space);

        var freedAllocation = new Allocation(allocation.Address, _blockFactory.CreateInvalid());

        var newAddress = CreateAddress(space);
        var newBlock = allocation.Block.Move(newAddress, size);
        var newAllocation = new Allocation(_nextAddress, newBlock);

        return (newAddress, new ConstantMemory(_alignment, _blockFactory, _collectionFactory,
            GetNextAddress(size), _allocations.SetItem(index, freedAllocation).Add(newAllocation)));
    }

    public IPersistentMemory Free(ISpace space, Section section, IExpression address)
    {
        var (index, allocation) = GetAllocation(space, address);

        if (!allocation.Block.CanFree(space, section, address))
            throw new StateException(StateError.InvalidMemoryFree, space);

        var freedAllocation = new Allocation(allocation.Address, _blockFactory.CreateInvalid());

        return new ConstantMemory(_alignment, _blockFactory, _collectionFactory,
            _nextAddress, _allocations.SetItem(index, freedAllocation));
    }

    public IPersistentMemory Write(ISpace space, IExpression address, IExpression value)
    {
        var newAllocations = new List<KeyValuePair<int, Allocation>>();

        while (true)
        {
            var (index, allocation) = GetAllocation(space, address);

            var block = address is IAddress a &&
                        allocation.Block is not AggregateBlock &&
                        allocation.Block.IsValid &&
                        allocation.Block.Size == a.IndexedType.Size &&
                        allocation.Address == (Bytes) (uint) a.BaseAddress.GetSingleValue(space)
                ? AggregateBlock.Create(_collectionFactory, space, allocation.Address, a.IndexedType, allocation.Block)
                : allocation.Block;

            var result = block.TryWrite(space, address, value);

            if (!result.CanBeSuccess)
                throw new StateException(StateError.InvalidMemoryWrite, space);

            newAllocations.Add(KeyValuePair.Create(index, new Allocation(allocation.Address, result.Value)));

            if (!result.CanBeFailure)
                return new ConstantMemory(_alignment, _blockFactory, _collectionFactory,
                    _nextAddress, _allocations.SetItems(newAllocations));

            space = result.FailureSpace;
        }
    }

    public IExpression Read(ISpace space, IExpression address, Bits size)
    {
        var expression = space.CreateZero(size);

        while (true)
        {
            var (_, allocation) = GetAllocation(space, address);
            var result = allocation.Block.TryRead(space, address, size);

            if (!result.CanBeSuccess)
                throw new StateException(StateError.InvalidMemoryRead, space);

            expression = expression.Or(result.Value);

            if (!result.CanBeFailure)
            {
                //if (!expression.IsConstant)
                    //Debugger.Break();

                return expression;
            }

            space = result.FailureSpace;
        }
    }

    private IExpression CreateAddress(ISpace space)
    {
        return space.CreateConstant(space.PointerSize, (uint) _nextAddress);
    }

    private Bytes GetNextAddress(Bits size)
    {
        return size == Bits.Zero
            ? _nextAddress + _alignment
            : (_nextAddress + size.ToBytes()).AlignTo(_alignment);
    }

    private (int, Allocation) GetAllocation(ISpace space, IExpression address)
    {
        var key = new Allocation((Bytes) (uint) address.GetExampleValue(space), _blockFactory.CreateInvalid());
        var result = _allocations.BinarySearch(key);

        var index = result < 0
            ? ~result - 1
            : result;

        return (index, _allocations.Get(index));
    }

    public static IPersistentMemory Create(Bytes alignment,
        IBlockFactory blockFactory, ICollectionFactory collectionFactory)
    {
        var nullAllocation = new Allocation(Bytes.Zero, blockFactory.CreateInvalid());

        return new ConstantMemory(alignment, blockFactory, collectionFactory,
            alignment, collectionFactory.CreatePersistentList<Allocation>().Add(nullAllocation));
    }

    private readonly struct Allocation : IComparable<Allocation>
    {
        public Allocation(Bytes address, IPersistentBlock block)
        {
            Address = address;
            Block = block;
        }

        public Bytes Address { get; }
        public IPersistentBlock Block { get; }

        public int CompareTo(Allocation other)
        {
            return Address.CompareTo(other.Address);
        }
    }
}
