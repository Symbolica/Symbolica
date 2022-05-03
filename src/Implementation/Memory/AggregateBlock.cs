using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal sealed class AggregateBlock : IPersistentBlock
{
    private readonly IPersistentList<Allocation> _allocations;

    private AggregateBlock(Section section, IExpression offset, Bytes size, IPersistentList<Allocation> allocations)
    {
        Section = section;
        Offset = offset;
        Size = size;
        _allocations = allocations;
    }

    public bool IsValid => true;
    public IExpression Offset { get; }
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

    public Result<IPersistentBlock> TryWrite(ICollectionFactory collectionFactory, ISpace space, IAddress address, IExpression value)
    {
        var isBaseAddressFullyInside = IsFullyInside(space, address.BaseAddress, value.Size.ToBytes());
        using var proposition = isBaseAddressFullyInside.GetProposition(space);

        if (proposition.CanBeFalse())
            // TODO: Handle both
            return Result<IPersistentBlock>.Failure(proposition.CreateFalseSpace());

        var relativeAddress = address.SubtractBase(space, Offset)
            ?? Address.Create(space.CreateZero(space.PointerSize));

        var (index, allocation) = Allocation.Get(space, relativeAddress.BaseAddress, _allocations);
        var result = allocation.Block.TryWrite(collectionFactory, space, relativeAddress, value);

        if (!result.CanBeFailure)
            return Result<IPersistentBlock>.Success(
                new AggregateBlock(
                    Section,
                    Offset,
                    Size,
                    _allocations.SetItem(index, new Allocation(allocation.Address, result.Value))));

        var isWholeAddressFullyInside = IsFullyInside(space, address, value.Size.ToBytes());
        using var proposition2 = isWholeAddressFullyInside.GetProposition(space);

        if (proposition2.CanBeFalse())
            return Result<IPersistentBlock>.Failure(proposition.CreateFalseSpace());

        var data = value.Size == Size.ToBits()
            ? value
            : Data(space).Write(space, GetOffset(space, relativeAddress), value);

        var allocations = _allocations.Select(
            a => new Allocation(
                a.Address,
                new PersistentBlock(
                    a.Block.Section,
                    a.Block.Offset,
                    data.Read(
                        space,
                        GetOffset(space, space.CreateConstant(space.PointerSize, (uint) a.Address)),
                        a.Block.Size.ToBits()))));

        return Result<IPersistentBlock>.Success(
            new AggregateBlock(
                Section,
                Offset,
                Size,
                collectionFactory.CreatePersistentList<Allocation>().AddRange(allocations)));
    }

    public Result<IExpression> TryRead(ISpace space, IAddress address, Bits size)
    {
        var isBaseAddressFullyInside = IsFullyInside(space, address.BaseAddress, size.ToBytes());
        using var proposition = isBaseAddressFullyInside.GetProposition(space);

        if (proposition.CanBeFalse())
            // TODO: Handle both
            return Result<IExpression>.Failure(proposition.CreateFalseSpace());

        var relativeAddress = address.SubtractBase(space, Offset)
            ?? Address.Create(space.CreateZero(space.PointerSize));

        var (_, allocation) = Allocation.Get(space, relativeAddress, _allocations);
        var result = allocation.Block.TryRead(space, relativeAddress, size);

        if (!result.CanBeFailure)
            return result;

        var isWholeAddressFullyInside = IsFullyInside(space, address, size.ToBytes());
        using var proposition2 = isWholeAddressFullyInside.GetProposition(space);

        if (proposition2.CanBeFalse())
            return Result<IExpression>.Failure(proposition.CreateFalseSpace());

        var value = size == Size.ToBits()
            ? Data(space)
            : Data(space).Read(space, GetOffset(space, relativeAddress), size);

        return Result<IExpression>.Success(value);
    }

    public IExpression Data(ISpace space)
    {
        return _allocations.Aggregate(
            space.CreateZero(Size.ToBits()),
            (buffer, alloc) =>
                buffer.Or(
                    alloc.Block.Data(space)
                        // Something in SQLite does a symbolic read from a buffer of addresses
                        // This "works", but then leads to dumb read on a mega struct
                        .ZeroExtend(Size.ToBits())
                        .ShiftLeft(GetOffset(space, alloc.Block.Offset))));
    }

    private IExpression IsFullyInside(ISpace space, IExpression address, Bytes size)
    {
        return address.UnsignedGreaterOrEqual(Offset)
            .And(GetBound(space, address, size).UnsignedLessOrEqual(GetBound(space, Offset, Size)));
    }

    private static IExpression GetBound(ISpace space, IExpression address, Bytes size)
    {
        return address.Add(space.CreateConstant(address.Size, (uint) size));
    }

    private IExpression GetOffset(ISpace space, IExpression address)
    {
        return space.CreateConstant(address.Size, (uint) Bytes.One.ToBits())
            .Multiply(address)
            .ZeroExtend(Size.ToBits())
            .Truncate(Size.ToBits());
    }

    private static IPersistentBlock SplitIndexedType(ICollectionFactory collectionFactory,
        ISpace space, Bytes address, IType indexedType, PersistentBlock block)
    {
        IType type = indexedType switch
        {
            IArrayType a => a.Resize(block.Size),
            _ => throw new Exception("Cant split a non address type.")
        };
        return Split(collectionFactory, space, address, type, block);
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
                        .Zip(type.Types, (o, t) =>
                            new Allocation(o, Split(collectionFactory, space, o, t, block.Read(space, o + address, t.Size))))))
            : block;
    }

    public static IPersistentBlock TryCreate(ICollectionFactory collectionFactory,
        ISpace space, IExpression address, Allocation allocation)
    {
        return address is IAddress a &&
               allocation.Block is PersistentBlock b &&
               allocation.Block.IsValid &&
               allocation.Address == (Bytes) (uint) a.BaseAddress.GetSingleValue(space)
            ? SplitIndexedType(collectionFactory, space, allocation.Address, a.IndexedType, b)
            : allocation.Block;
    }
}
