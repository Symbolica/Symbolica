using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Implementation.Memory;

internal sealed class SymbolicMemory : IPersistentMemory
{
    private readonly Bytes _alignment;
    private readonly IBlockFactory _blockFactory;
    private readonly IPersistentList<IPersistentBlock> _blocks;

    private SymbolicMemory(Bytes alignment, IBlockFactory blockFactory,
        IPersistentList<IPersistentBlock> blocks)
    {
        _alignment = alignment;
        _blockFactory = blockFactory;
        _blocks = blocks;
    }

    public (Address, IPersistentMemory) Allocate(ISpace space, Section section, Bits size)
    {
        var address = CreateAddress(space, size.ToBytes());
        var block = _blockFactory.Create(space, section, address, size);

        return (address, new SymbolicMemory(_alignment, _blockFactory,
            _blocks.Add(block)));
    }

    public (Address, IPersistentMemory) Move(ISpace space, Section section, Address address, Bits size)
    {
        foreach (var (block, index) in _blocks.Select((b, i) => (b, i)))
            if (block.CanFree(space, section, address))
            {
                var newAddress = CreateAddress(space, size.ToBytes());
                var newBlock = block.Move(newAddress, size);

                return (newAddress, new SymbolicMemory(_alignment, _blockFactory,
                    _blocks.SetItem(index, newBlock)));
            }

        throw new StateException(StateError.InvalidMemoryMove, space);
    }

    public IPersistentMemory Free(ISpace space, Section section, Address address)
    {
        foreach (var (block, index) in _blocks.Select((b, i) => (b, i)))
            if (block.CanFree(space, section, address))
                return new SymbolicMemory(_alignment, _blockFactory,
                    _blocks.SetItem(index, _blockFactory.CreateInvalid()));

        throw new StateException(StateError.InvalidMemoryFree, space);
    }

    public IPersistentMemory Write(ISpace space, Address address, IExpression<IType> value)
    {
        var newBlocks = new List<KeyValuePair<int, IPersistentBlock>>();

        foreach (var (block, index) in _blocks.Select((b, i) => (b, i)))
        {
            var result = block.TryWrite(space, address, value);

            if (!result.CanBeSuccess)
                continue;

            newBlocks.Add(KeyValuePair.Create(index, result.Value));

            if (!result.CanBeFailure)
                return new SymbolicMemory(_alignment, _blockFactory,
                    _blocks.SetItems(newBlocks));

            space = result.FailureSpace;
        }

        throw new StateException(StateError.InvalidMemoryWrite, space);
    }

    public IExpression<IType> Read(ISpace space, Address address, Bits size)
    {
        var expression = ConstantUnsigned.CreateZero(size) as IExpression<IType>;

        foreach (var block in _blocks)
        {
            var result = block.TryRead(space, address, size);

            if (!result.CanBeSuccess)
                continue;

            expression = Or.Create(expression, result.Value);

            if (!result.CanBeFailure)
                return expression;

            space = result.FailureSpace;
        }

        throw new StateException(StateError.InvalidMemoryRead, space);
    }

    private Address CreateAddress(ISpace space, Bytes size)
    {
        return Address.Create(
            Symbol.Create(
                space.PointerSize,
                null,
                _blocks
                    .Where(b => b.IsValid)
                    .Select(b => new Func<IExpression<IType>, IExpression<IType>>(a => IsFullyOutside(b, a, size)))
                    .Append(a => NotEqual.Create(a, ConstantUnsigned.CreateZero(a.Size)))
                    .Append(a => UnsignedLessOrEqual.Create(a, GetBound(a, size)))
                    .Append(a => Equal.Create(
                        UnsignedRemainder.Create(a, ConstantUnsigned.Create(a.Size, (uint) _alignment)),
                        ConstantUnsigned.CreateZero(a.Size)))));
    }

    private static IExpression<IType> IsFullyOutside(IPersistentBlock block, IExpression<IType> address, Bytes size)
    {
        return Or.Create(
            UnsignedLessOrEqual.Create(GetBound(address, size), block.Address),
            UnsignedGreaterOrEqual.Create(address, GetBound(block.Address, block.Size)));
    }

    private static IExpression<IType> GetBound(IExpression<IType> address, Bytes size)
    {
        return Add.Create(address, ConstantUnsigned.Create(address.Size, (uint) size));
    }

    public static IPersistentMemory Create(Bytes alignment,
        IBlockFactory blockFactory, ICollectionFactory collectionFactory)
    {
        return new SymbolicMemory(alignment, blockFactory,
            collectionFactory.CreatePersistentList<IPersistentBlock>());
    }
}
