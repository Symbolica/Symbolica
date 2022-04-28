using System;
using System.Diagnostics;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal sealed class AggregateBlock : IPersistentBlock
{
    private readonly IPersistentList<Allocation> _allocations;

    private AggregateBlock(Section section, IExpression address, Bytes size, IPersistentList<Allocation> allocations)
    {
        Section = section;
        Address = address;
        Size = size;
        _allocations = allocations;
    }

    public bool IsValid => true;
    public IExpression Address { get; }
    public Bytes Size { get; }

    public Section Section { get; }

    public IPersistentBlock Move(IExpression address, Bits size)
    {
        throw new NotImplementedException();
    }

    public bool CanFree(ISpace space, Section section, IExpression address)
    {
        return true;
    }

    public Result<IPersistentBlock> TryWrite(ISpace space, IExpression address, IExpression value)
    {
        if (value.Size == Size.ToBits())
            return Result<IPersistentBlock>.Success(new PersistentBlock(Section, Address, value));

        var isFullyInside = IsFullyInside(space, address, value.Size.ToBytes());
        using var proposition = isFullyInside.GetProposition(space);

        if (proposition.CanBeFalse())
            return Result<IPersistentBlock>.Failure(proposition.CreateFalseSpace());

        var (index, allocation) = Allocation.Get(space, address, _allocations);

        var result = allocation.Block.TryWrite(space, address, value);

        return result.CanBeFailure
            ? Result<IPersistentBlock>.Success(
                new PersistentBlock(
                    Section,
                    Address,
                    Data(space).Write(space, GetOffset(space, address), value)))
            : Result<IPersistentBlock>.Success(
                new AggregateBlock(
                    Section,
                    Address,
                    Size,
                    _allocations.SetItem(index, new Allocation(allocation.Address, result.Value))));
    }

    public Result<IExpression> TryRead(ISpace space, IExpression address, Bits size)
    {
        if (size == Size.ToBits())
            return Result<IExpression>.Success(Data(space));

        var isFullyInside = IsFullyInside(space, address, size.ToBytes());
        using var proposition = isFullyInside.GetProposition(space);

        if (proposition.CanBeFalse())
            // TODO: Handle both
            return Result<IExpression>.Failure(proposition.CreateFalseSpace());

        var (_, allocation) = Allocation.Get(space, address, _allocations);

        var result = allocation.Block.TryRead(space, address, size);

        return result.CanBeFailure
            ? Result<IExpression>.Success(
                Data(space).Read(space, GetOffset(space, address), size))
            : result;
    }

    public IExpression Data(ISpace space)
    {
        return _allocations.Aggregate(
            space.CreateZero(Size.ToBits()),
            (buffer, alloc) =>
                buffer.Or(
                        alloc.Block.Data(space)
                            .ZeroExtend(Size.ToBits())
                            .ShiftLeft(GetOffset(space, alloc.Block.Address))));
    }

    private IExpression IsFullyInside(ISpace space, IExpression address, Bytes size)
    {
        return address.UnsignedGreaterOrEqual(Address)
            .And(GetBound(space, address, size).UnsignedLessOrEqual(GetBound(space, Address, Size)));
    }

    private static IExpression GetBound(ISpace space, IExpression address, Bytes size)
    {
        return address.Add(space.CreateConstant(address.Size, (uint) size));
    }

    private IExpression GetOffset(ISpace space, IExpression address)
    {
        var offset = space.CreateConstant(address.Size, (uint) Bytes.One.ToBits())
            .Multiply(address is IAddress a
                ? a.SubtractBase(Address)
                : address.Subtract(Address));

        return offset.ZeroExtend(Size.ToBits()).Truncate(Size.ToBits());
    }

    private static IPersistentBlock Split(ICollectionFactory collectionFactory,
        ISpace space, Bytes address, IType type, PersistentBlock block)
    {
        return type.Types.Any()
            ? new AggregateBlock(
                block.Section,
                space.CreateConstant(space.PointerSize, (uint) address),
                type.Size,
                collectionFactory.CreatePersistentList<Allocation>()
                    .AddRange(type.Offsets
                        .Select(o => address + o)
                        .Zip(type.Types, (a, t) =>
                            new Allocation(a, Split(collectionFactory, space, a, t, block.Read(space, a, t.Size))))))
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
            ? Split(collectionFactory, space, allocation.Address, a.IndexedType, b)
            : allocation.Block;
    }
}
