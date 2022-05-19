using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal sealed class SystemProxy : ISystemProxy
{
    private IPersistentSystem _system;

    public SystemProxy(IPersistentSystem system)
    {
        _system = system;
    }

    public ISystemProxy Clone()
    {
        return new SystemProxy(_system);
    }

    public IExpression GetThreadAddress(ISpace space, IMemory memory)
    {
        var (address, system) = _system.GetThreadAddress(space, memory);
        _system = system;

        return address;
    }

    public int Open(string path)
    {
        var (descriptor, system) = _system.Open(path);
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

    public int Read(ISpace space, IMemory memory, int descriptor, IExpression address, int count)
    {
        var result = _system.Read(space, memory, descriptor, address, count);

        if (result > 0)
            (_, _system) = _system.Seek(descriptor, result, 1U);

        return result;
    }

    public IExpression ReadDirectory(ISpace space, IMemory memory, IExpression address)
    {
        return _system.ReadDirectory(space, memory, address);
    }

    public int GetStatus(ISpace space, IMemory memory, int descriptor, IExpression address)
    {
        return _system.GetStatus(space, memory, descriptor, address);
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(ISystemProxy other)
    {
        return other is SystemProxy sp
            ? _system.IsEquivalentTo(sp._system)
            : (new(), false);
    }

    public object ToJson()
    {
        return _system.ToJson();
    }
}
