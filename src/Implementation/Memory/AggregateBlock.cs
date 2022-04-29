using System;
using System.Linq;
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

    public Result<IPersistentBlock> TryWrite(ISpace space, IAddress address, IExpression value)
    {
        var isBaseAddressFullyInside = IsFullyInside(space, address.BaseAddress, value.Size.ToBytes());
        using var proposition = isBaseAddressFullyInside.GetProposition(space);

        if (proposition.CanBeFalse())
            // TODO: Handle both
            return Result<IPersistentBlock>.Failure(proposition.CreateFalseSpace());

        // if (value.Size == Size.ToBits())
        //     return Result<IPersistentBlock>.Success(new PersistentBlock(Section, Offset, value));

        var nextAddress = NextAddress(space, address);
        if (nextAddress is null)
            // TODO: This could happen if there are implicit zeros at the end of a GEP
            throw new Exception("Ran out of offsets before we hit the target field.");

        var (index, allocation) = Allocation.Get(space, nextAddress.BaseAddress, _allocations);
        var result = allocation.Block.TryWrite(space, nextAddress, value);

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

        return Result<IPersistentBlock>.Success(
            new PersistentBlock(
                Section,
                Offset,
                Data(space).Write(space, GetOffset(space, nextAddress), value)));
    }

    public Result<IExpression> TryRead(ISpace space, IAddress address, Bits size)
    {
        var isBaseAddressFullyInside = IsFullyInside(space, address.BaseAddress, size.ToBytes());
        using var proposition = isBaseAddressFullyInside.GetProposition(space);

        if (proposition.CanBeFalse())
            // TODO: Handle both
            return Result<IExpression>.Failure(proposition.CreateFalseSpace());

        // if (size == Size.ToBits())
        //     return Result<IExpression>.Success(Data(space));

        var nextAddress = NextAddress(space, address);
        if (nextAddress is null)
            // TODO: This could happen if there are implicit zeros at the end of a GEP
            throw new Exception("Ran out of offsets before we hit the target field.");

        var (_, allocation) = Allocation.Get(space, nextAddress, _allocations);
        var result = allocation.Block.TryRead(space, nextAddress, size);

        if (!result.CanBeFailure)
            return result;

        var isWholeAddressFullyInside = IsFullyInside(space, address, size.ToBytes());
        using var proposition2 = isWholeAddressFullyInside.GetProposition(space);

        if (proposition2.CanBeFalse())
            return Result<IExpression>.Failure(proposition.CreateFalseSpace());

        return Result<IExpression>.Success(
            Data(space).Read(space, GetOffset(space, nextAddress), size));
    }

    private IAddress? NextAddress(ISpace space, IAddress address)
    {
        IAddress? SkipPointers(IType parentType, IAddress? address)
        {
            bool IsZero(IExpression expression)
            {
                var isZero = expression.Equal(space.CreateZero(expression.Size));
                using var proposition = isZero.GetProposition(space);
                return !proposition.CanBeFalse();
            }

            return address is not null && parentType is IPointerType p && address.IndexedType.Size == Size && IsZero(address.BaseAddress)
                ? SkipPointers(address.IndexedType, address.Tail())
                : address;
        }

        var next = address.SubtractBase(space, Offset);
        return SkipPointers(address.IndexedType, next);
    }

    public IExpression Data(ISpace space)
    {
        return _allocations.Aggregate(
            space.CreateZero(Size.ToBits()),
            (buffer, alloc) =>
                buffer.Or(
                        alloc.Block.Data(space)
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
        var invalidSize = indexedType switch
        {
            IPointerType pointer => pointer.ElementType.Size > block.Size,
            _ => indexedType.Size != block.Size
        };
        if (invalidSize)
            throw new Exception("Size problem when spitting block.");

        IType type = indexedType switch
        {
            IPointerType pointer => pointer.Deferefence(block.Size),
            _ => indexedType
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
