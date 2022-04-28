using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal sealed class AggregateBlock : IPersistentBlock
{
    private readonly IPersistentList<Allocation> _allocations;
    private readonly IPersistentBlock _block;

    private AggregateBlock(IPersistentBlock block, IPersistentList<Allocation> allocations)
    {
        _block = block;
        _allocations = allocations;
    }

    public bool IsValid => _block.IsValid;
    public IExpression Address => _block.Address;
    public Bytes Size => _block.Size;

    public IPersistentBlock Move(IExpression address, Bits size)
    {
        return _block.Move(address, size);
    }

    public bool CanFree(ISpace space, Section section, IExpression address)
    {
        return _block.CanFree(space, section, address);
    }

    public Result<IPersistentBlock> TryWrite(ISpace space, IExpression address, IExpression value)
    {
        var result = _block.TryWrite(space, address, value);

        if (result.CanBeFailure)
            return result;

        var (index, allocation) = Allocation.Get(space, address, _allocations);

        var result2 = allocation.Block.TryWrite(space, address, value);

        return result2.CanBeFailure
            ? result
            : Result<IPersistentBlock>.Success(new AggregateBlock(result.Value,
                _allocations.SetItem(index, new Allocation(allocation.Address, result2.Value))));
    }

    public Result<IExpression> TryRead(ISpace space, IExpression address, Bits size)
    {
        var result = _block.TryRead(space, address, size);

        if (result.CanBeFailure)
            return result;

        var (_, allocation) = Allocation.Get(space, address, _allocations);

        var result2 = allocation.Block.TryRead(space, address, size);

        return result2.CanBeFailure
            ? result
            : result2;
    }

    private static IPersistentBlock TrySplit(ICollectionFactory collectionFactory,
        ISpace space, Bytes address, IType type, PersistentBlock block)
    {
        return type.Types.Any()
            ? new AggregateBlock(block, collectionFactory.CreatePersistentList<Allocation>()
                .AddRange(type.Offsets
                    .Select(o => address + o)
                    .Zip(type.Types, (a, t) =>
                        new Allocation(a, TrySplit(collectionFactory, space, a, t, block.Read(space, a, t.Size))))))
            : block;
    }

    public static IPersistentBlock TryCreate(ICollectionFactory collectionFactory,
        ISpace space, IExpression address, Allocation allocation)
    {
        return address is IAddress a &&
               allocation.Block is PersistentBlock b &&
               allocation.Block.IsValid &&
               allocation.Block.Size == a.IndexedType.Size &&
               allocation.Address == (Bytes) (uint) a.BaseAddress.GetSingleValue(space)
            ? TrySplit(collectionFactory, space, allocation.Address, a.IndexedType, b)
            : allocation.Block;
    }
}
