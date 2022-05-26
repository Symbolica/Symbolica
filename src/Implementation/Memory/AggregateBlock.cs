using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Abstraction.Memory;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal sealed class AggregateBlock : IPersistentBlock
{
    private readonly IExpressionFactory _exprFactory;
    private readonly IPersistentList<Allocation> _allocations;
    private readonly Lazy<int> _equivalencyHash;
    private readonly Lazy<int> _mergeHash;

    private AggregateBlock(IExpressionFactory exprFactory, Section section,
        IExpression offset, Bytes size, IPersistentList<Allocation> allocations)
    {
        _exprFactory = exprFactory;
        Section = section;
        Offset = offset;
        Size = size;
        _allocations = allocations;
        _equivalencyHash = new(() => EquivalencyHash(false));
        _mergeHash = new(() => EquivalencyHash(true));

        int EquivalencyHash(bool includeSubs)
        {
            var allocationsHash = new HashCode();
            foreach (var allocation in _allocations)
                allocationsHash.Add(allocation.GetEquivalencyHash(includeSubs));

            return HashCode.Combine(
                Section,
                Offset.GetEquivalencyHash(includeSubs),
                Size,
                allocationsHash.ToHashCode());
        }
    }

    public bool IsValid => true;
    public IExpression Offset { get; }
    public Bytes Size { get; }
    public Section Section { get; }

    public IExpression Data =>
        _allocations.Aggregate(
            _exprFactory.CreateZero(Size.ToBits()),
            (buffer, alloc) =>
                buffer.Or(
                    alloc.Block.Data
                        // Something in SQLite does a symbolic read from a buffer of addresses
                        // This "works", but then leads to dumb read on a mega struct
                        .ZeroExtend(Size.ToBits())
                        .ShiftLeft(GetOffset(alloc.Block.Offset))));

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
        var isBaseAddressFullyInside = IsFullyInside(address.BaseAddress, value.Size.ToBytes());
        using var proposition = isBaseAddressFullyInside.GetProposition(space);

        if (proposition.CanBeFalse())
            // TODO: Handle both
            return Result<IPersistentBlock>.Failure(proposition.CreateFalseSpace());

        var relativeAddress = address.SubtractBase(space, Offset)
            ?? Address.Create(_exprFactory, _exprFactory.CreateZero(_exprFactory.PointerSize));

        var (index, allocation) = Allocation.Get(space, relativeAddress.BaseAddress, _allocations);
        var result = allocation.Block.TryWrite(space, relativeAddress, value);

        if (!result.CanBeFailure)
            return Result<IPersistentBlock>.Success(
                new AggregateBlock(
                    _exprFactory,
                    Section,
                    Offset,
                    Size,
                    _allocations.SetItem(index, new Allocation(allocation.Address, result.Value))));

        // if (value.Size.ToBytes() > allocation.Block.Size)
        //     Debugger.Break();
        var isWholeAddressFullyInside = IsFullyInside(address, value.Size.ToBytes());
        using var proposition2 = isWholeAddressFullyInside.GetProposition(space);

        if (proposition2.CanBeFalse())
            return Result<IPersistentBlock>.Failure(proposition.CreateFalseSpace());

        var data = value.Size == Size.ToBits()
            ? value
            : Data.Write(space, GetOffset(relativeAddress), value);

        // if (data.IsSymbolic)
        //     Debugger.Break();

        return Result<IPersistentBlock>.Success(new PersistentBlock(_exprFactory, Section, Offset, data));
    }

    public Result<IExpression> TryRead(ISpace space, IAddress address, Bits size)
    {
        var isBaseAddressFullyInside = IsFullyInside(address.BaseAddress, size.ToBytes());
        using var proposition = isBaseAddressFullyInside.GetProposition(space);

        if (proposition.CanBeFalse())
            // TODO: Handle both
            return Result<IExpression>.Failure(proposition.CreateFalseSpace());

        var relativeAddress = address.SubtractBase(space, Offset)
            ?? Address.Create(_exprFactory, _exprFactory.CreateZero(_exprFactory.PointerSize));

        var (_, allocation) = Allocation.Get(space, relativeAddress, _allocations);
        var result = allocation.Block.TryRead(space, relativeAddress, size);

        if (!result.CanBeFailure)
            return result;

        var isWholeAddressFullyInside = IsFullyInside(address, size.ToBytes());
        using var proposition2 = isWholeAddressFullyInside.GetProposition(space);

        if (proposition2.CanBeFalse())
            return Result<IExpression>.Failure(proposition.CreateFalseSpace());

        var value = size == Size.ToBits()
            ? Data
            : Data.Read(space, GetOffset(relativeAddress), size);

        if (value.IsSymbolic)
            Debugger.Break();

        return Result<IExpression>.Success(value);
    }

    private IExpression IsFullyInside(IExpression address, Bytes size)
    {
        return address.UnsignedGreaterOrEqual(Offset)
            .And(GetBound(address, size).UnsignedLessOrEqual(GetBound(Offset, Size)));
    }

    private IExpression GetBound(IExpression address, Bytes size)
    {
        return address.Add(_exprFactory.CreateConstant(address.Size, (uint) size));
    }

    private IExpression GetOffset(IExpression address)
    {
        return _exprFactory.CreateConstant(address.Size, (uint) Bytes.One.ToBits())
            .Multiply(address)
            .ZeroExtend(Size.ToBits())
            .Truncate(Size.ToBits());
    }

    private static IPersistentBlock SplitIndexedType(ICollectionFactory collectionFactory, IExpressionFactory exprFactory,
        ISpace space, Bytes address, IType indexedType, PersistentBlock block)
    {
        IType type = indexedType switch
        {
            IArrayType a => a.Resize(block.Size),
            _ => throw new Exception("Cant split a non address type.")
        };
        return Split(collectionFactory, exprFactory, space, address, type, block);
    }

    private static IPersistentBlock Split(ICollectionFactory collectionFactory, IExpressionFactory exprFactory,
        ISpace space, Bytes address, IType type, PersistentBlock block)
    {
        return type.Types.Any()
            ? new AggregateBlock(
                exprFactory,
                block.Section,
                exprFactory.CreateConstant(exprFactory.PointerSize, (uint) address),
                type.Size,
                collectionFactory.CreatePersistentList<Allocation>()
                    .AddRange(type.Offsets
                        .Zip(type.Types, (o, t) =>
                            new Allocation(
                                o,
                                Split(collectionFactory, exprFactory, space, o, t, block.Read(space, o + address, t.Size))))))
            : block;
    }

    public static IPersistentBlock TryCreate(ICollectionFactory collectionFactory,
        IExpressionFactory exprFactory, ISpace space, IExpression address, Allocation allocation)
    {
        return address is IAddress a &&
               allocation.Block is PersistentBlock b &&
               allocation.Block.IsValid &&
               allocation.Address == (Bytes) (uint) a.BaseAddress.GetSingleValue(space)
            ? SplitIndexedType(collectionFactory, exprFactory, space, allocation.Address, a.IndexedType, b)
            : allocation.Block;
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IPersistentBlock other)
    {
        return other is AggregateBlock b
            ? Offset.IsEquivalentTo(b.Offset).ToHashSet()
                .And((new(), Section == b.Section))
                .And((new(), Size == b.Size))
                .And(_allocations.IsSequenceEquivalentTo<ExpressionSubs, Allocation>(b._allocations))
            : (new(), false);
    }

    public object ToJson()
    {
        return new
        {
            Offset = Offset.ToJson(),
            Section = Section.ToString(),
            Size = (uint) Size,
            Allocations = _allocations.Select(a => a.ToJson()).ToArray()
        };
    }

    public int GetEquivalencyHash(bool includeSubs)
    {
        return includeSubs
            ? _mergeHash.Value
            : _equivalencyHash.Value;
    }
}
