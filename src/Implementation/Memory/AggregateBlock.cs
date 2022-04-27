using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Collection;
using Symbolica.Expression;
using Symbolica.Implementation.Exceptions;

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

        return result.CanBeSuccess
            ? result.CanBeFailure
                ? Result<IPersistentBlock>.Both(result.FailureSpace, new AggregateBlock(result.Value, Write(space, address, value)))
                : Result<IPersistentBlock>.Success(new AggregateBlock(result.Value, Write(space, address, value)))
            : result;
    }

    public Result<IExpression> TryRead(ISpace space, IExpression address, Bits size)
    {
        var result = _block.TryRead(space, address, size);

        return result.CanBeSuccess
            ? result.CanBeFailure
                ? Result<IExpression>.Both(result.FailureSpace, Read(space, address, size))
                : Result<IExpression>.Success(Read(space, address, size))
            : result;
    }

    private IPersistentList<Allocation> Write(ISpace space, IExpression address, IExpression value)
    {
        var allocations = GetOverlappingAllocations(space, address, value.Size.ToBytes());

        var firstAllocation = allocations.First().Item2;
        var lastAllocation = allocations.Last().Item2;

        if (allocations.Count == 1 && firstAllocation.Size == value.Size.ToBytes())
        {
            return _allocations.SetItem(allocations.First().Item1, new Allocation(firstAllocation.Address, firstAllocation.Size, value));
        }

        var tempSize = (lastAllocation.Address + lastAllocation.Size - firstAllocation.Address).ToBits();

        var expression = tempSize == value.Size
            ? value
            : allocations.Aggregate(space.CreateZero(tempSize), (e, a) =>
                e.Or(a.Item2.Data.ZeroExtend(tempSize).Truncate(tempSize).ShiftLeft(space.CreateConstant(e.Size, (uint) (a.Item2.Address - firstAllocation.Address)))))
            .Write(address.Subtract(space.CreateConstant(address.Size, (uint) firstAllocation.Address)).ZeroExtend(tempSize).Truncate(tempSize), value);

        if (!expression.IsConstant)
            Debugger.Break();

        return _allocations.SetItems(allocations.Select(a => new KeyValuePair<int, Allocation>(a.Item1, new Allocation(a.Item2.Address, a.Item2.Size,
            TryGetSingleValue(space, expression.Read(space.CreateConstant(expression.Size, (uint) (a.Item2.Address - firstAllocation.Address)), a.Item2.Size.ToBits()))))));
    }

    private IExpression Read(ISpace space, IExpression address, Bits size)
    {
        var allocations = GetOverlappingAllocations(space, address, size.ToBytes());

        var firstAllocation = allocations.First().Item2;
        var lastAllocation = allocations.Last().Item2;

        if (allocations.Count == 1 && firstAllocation.Size == size.ToBytes())
        {
            return TryGetSingleValue(space, firstAllocation.Data);
        }

        var tempSize = (lastAllocation.Address + lastAllocation.Size - firstAllocation.Address).ToBits();

        var expression = allocations.Aggregate(space.CreateZero(tempSize), (e, a) =>
            e.Or(a.Item2.Data.ZeroExtend(tempSize).Truncate(tempSize).ShiftLeft(space.CreateConstant(e.Size, (uint) (a.Item2.Address - firstAllocation.Address)))));

        if (!expression.IsConstant)
            Debugger.Break();

        var tryGetSingleValue = TryGetSingleValue(space, tempSize == size
            ? expression
            : expression.Read(
                address.Subtract(space.CreateConstant(address.Size, (uint) firstAllocation.Address))
                    .ZeroExtend(tempSize).Truncate(tempSize), size));

        if (!tryGetSingleValue.IsConstant)
            Debugger.Break();

        return tryGetSingleValue;
    }

    private static IExpression TryGetSingleValue(ISpace space, IExpression expression)
    {
        var value = space.CreateConstant(expression.Size, expression.GetExampleValue(space));

        var comparison = expression.Equal(value);

        using var proposition = comparison.GetProposition(space);

        return proposition.CanBeFalse()
            ? expression
            : value;
    }

    private static bool IsOverlapping(ISpace space, IExpression address, Bytes size, Allocation allocation)
    {
        var isOverlapping = address
            .UnsignedLess(space.CreateConstant(address.Size,
                (uint) (allocation.Address + allocation.Size)))
            .And(address.Add(space.CreateConstant(address.Size, (uint) size))
                .UnsignedGreater(space.CreateConstant(address.Size, (uint) allocation.Address)));

        using var proposition = isOverlapping.GetProposition(space);

        return proposition.CanBeTrue();
    }

    private List<(int, Allocation)> GetOverlappingAllocations(ISpace space, IExpression address, Bytes size)
    {
        var set = new HashSet<int>();
        var allocations = new List<(int, Allocation)>();

        foreach (var (baseIndex, _) in GetBaseAllocations(space, address).OrderBy(i => i.Item1))
        {
            var index = baseIndex;

            do
            {
                if (!set.Add(index))
                    break;

                var allocation = _allocations.Get(index);
                allocations.Add((index, allocation));
                ++index;

            } while (index < _allocations.Count && IsOverlapping(space, address, size, _allocations.Get(index)));
        }

        return allocations;
    }

    private IEnumerable<(int, Allocation)> GetBaseAllocations(ISpace space, IExpression address)
    {
        while (true)
        {
            var (index, allocation) = GetAllocation(space, address);

            var isInside = address.UnsignedGreaterOrEqual(space.CreateConstant(address.Size, (uint) allocation.Address))
                .And(address.UnsignedLess(space.CreateConstant(address.Size, (uint) (allocation.Address + allocation.Size))));

            using var proposition = isInside.GetProposition(space);

            //if (!proposition.CanBeTrue())
                //throw new Exception("remove this check in a bit");

            yield return (index, allocation);

            if (!proposition.CanBeFalse())
                yield break;

            space = proposition.CreateFalseSpace();
        }
    }

    private (int, Allocation) GetAllocation(ISpace space, IExpression address)
    {
        var key = new Allocation((Bytes) (uint) address.GetExampleValue(space), Bytes.One, space.CreateZero(Bytes.One.ToBits()));
        var result = _allocations.BinarySearch(key);

        var index = result < 0
            ? ~result - 1
            : result;

        return (index, _allocations.Get(index));
    }

    private static IEnumerable<Allocation> Split(ISpace space, Bytes address, IType type, IPersistentBlock block)
    {
        return type.Types.Any()
            ? type.Types
                .Zip(type.Offsets, (t, o) => (t, o))
                .SelectMany(e => Split(space, address + e.o, e.t, block))
            : new[] { Allocation.Create(space, address, type.Size, block) };
    }

    public static IPersistentBlock Create(ICollectionFactory collectionFactory,
        ISpace space, Bytes address, IType type, IPersistentBlock block)
    {
        return new AggregateBlock(block, collectionFactory.CreatePersistentList<Allocation>()
            .AddRange(Split(space, address, type, block)));
    }

    private readonly struct Allocation : IComparable<Allocation>
    {
        public Allocation(Bytes address, Bytes size, IExpression data)
        {
            Address = address;
            Size = size;
            Data = data;
        }

        public Bytes Address { get; }
        public Bytes Size { get; }
        public IExpression Data { get; }

        public int CompareTo(Allocation other)
        {
            return Address.CompareTo(other.Address);
        }

        public static Allocation Create(ISpace space, Bytes address, Bytes size, IPersistentBlock block)
        {
            var result = block.TryRead(space, space.CreateConstant(block.Address.Size, (uint) address), size.ToBits());

            // remove check ?
            if (result.CanBeFailure)
                throw new ImplementationException("wtf");

            return new Allocation(address, size, result.Value);
        }
    }
}
