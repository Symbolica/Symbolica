using System;
using System.Collections.Generic;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal readonly struct Allocation : IComparable<Allocation>, IMergeable<ExpressionSubs, Allocation>
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

    public static (int, Allocation) Get(ISpace space, IExpression address, IPersistentList<Allocation> allocations)
    {
        var key = new Allocation((Bytes) (uint) address.GetExampleValue(space), null!);
        var result = allocations.BinarySearch(key);

        var index = result < 0
            ? ~result - 1
            : result;

        return (index, allocations.Get(index));
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(Allocation other)
    {
        return (new HashSet<ExpressionSubs>(), Address == other.Address)
            .And(Block.IsEquivalentTo(other.Block));
    }

    public int GetEquivalencyHash(bool includeSubs)
    {
        return HashCode.Combine(Address, Block.GetEquivalencyHash(includeSubs));
    }

    public object ToJson()
    {
        return new
        {
            Address = (uint) Address,
            Block = Block.ToJson()
        };
    }
}
