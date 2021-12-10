using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.System;

internal sealed class SystemProxy : ISystemProxy
{
    private readonly IMemoryProxy _memory;
    private readonly ISpace _space;
    private IPersistentSystem _system;

    public SystemProxy(ISpace space, IMemoryProxy memory, IPersistentSystem system)
    {
        _space = space;
        _memory = memory;
        _system = system;
    }

    public ISystemProxy Clone(ISpace space, IMemoryProxy memory)
    {
        return new SystemProxy(space, memory, _system);
    }

    public IExpression GetThreadAddress()
    {
        var (address, system) = _system.GetThreadAddress(_space, _memory);
        _system = system;

        return address;
    }

    public int Open(IExpression path)
    {
        var (descriptor, system) = _system.Open(_space, _memory, path);
        _system = system;

        return descriptor;
    }

    public int Duplicate(int descriptor)
    {
        var (result, system) = _system.Duplicate(descriptor);
        _system = system;

        return result;
    }

    public int Close(int descriptor)
    {
        var (result, system) = _system.Close(descriptor);
        _system = system;

        return result;
    }

    public long Seek(int descriptor, long offset, uint whence)
    {
        var (result, system) = _system.Seek(descriptor, offset, whence);
        _system = system;

        return result;
    }

    public int Read(int descriptor, IExpression address, int count)
    {
        var result = _system.Read(_space, _memory, descriptor, address, count);

        if (result > 0)
            (_, _system) = _system.Seek(descriptor, result, 1U);

        return result;
    }

    public IExpression ReadDirectory(IExpression address)
    {
        return _system.ReadDirectory(_space, _memory, address);
    }

    public int GetStatus(int descriptor, IExpression address)
    {
        return _system.GetStatus(_space, _memory, descriptor, address);
    }
}
