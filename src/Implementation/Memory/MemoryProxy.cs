using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Implementation.Memory;

internal sealed class MemoryProxy : IMemoryProxy
{
    private readonly ISpace _space;
    private IPersistentMemory _memory;

    public MemoryProxy(ISpace space, IPersistentMemory memory)
    {
        _space = space;
        _memory = memory;
    }

    public IMemoryProxy Clone(ISpace space)
    {
        return new MemoryProxy(space, _memory);
    }

    public Address Allocate(Section section, Bits size)
    {
        var (address, memory) = _memory.Allocate(_space, section, size);
        _memory = memory;

        return address;
    }

    public void Free(Section section, Address address)
    {
        _memory = _memory.Free(_space, section, address);
    }

    public Address Allocate(Bits size)
    {
        return Allocate(Section.Heap, size);
    }

    public Address Move(Address address, Bits size)
    {
        var (newAddress, memory) = _memory.Move(_space, Section.Heap, address, size);
        _memory = memory;

        return newAddress;
    }

    public void Free(Address address)
    {
        Free(Section.Heap, address);
    }

    public void Write(Address address, IExpression<IType> value)
    {
        _memory = _memory.Write(_space, address, value);
    }

    public IExpression<IType> Read(Address address, Bits size)
    {
        return _memory.Read(_space, address, size);
    }
}
