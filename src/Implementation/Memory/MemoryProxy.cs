using System.Collections.Generic;
using Symbolica.Abstraction.Memory;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal sealed class MemoryProxy : IMemoryProxy
{
    private IPersistentMemory _memory;

    public MemoryProxy(IPersistentMemory memory)
    {
        _memory = memory;
    }

    public IMemoryProxy Clone()
    {
        return new MemoryProxy(_memory);
    }

    public IExpression Allocate(Section section, Bits size)
    {
        var (address, memory) = _memory.Allocate(section, size);
        _memory = memory;

        return address;
    }

    public IExpression Allocate(Bits size)
    {
        return Allocate(Section.Heap, size);
    }

    public IExpression Move(ISpace space, IExpression address, Bits size)
    {
        var (newAddress, memory) = _memory.Move(space, Section.Heap, address, size);
        _memory = memory;

        return newAddress;
    }

    public void Free(ISpace space, IExpression address)
    {
        Free(space, Section.Heap, address);
    }

    public void Free(ISpace space, Section section, IExpression address)
    {
        _memory = _memory.Free(space, section, address);
    }

    public void Write(ISpace space, IExpression address, IExpression value)
    {
        _memory = _memory.Write(space, address, value);
    }

    public IExpression Read(ISpace space, IExpression address, Bits size)
    {
        return _memory.Read(space, address, size);
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(IMemoryProxy other)
    {
        return other is MemoryProxy mp
            ? _memory.IsEquivalentTo(mp._memory)
            : (new(), false);
    }

    public object ToJson()
    {
        return _memory.ToJson();
    }

    public int GetEquivalencyHash()
    {
        return _memory.GetEquivalencyHash();
    }
}
