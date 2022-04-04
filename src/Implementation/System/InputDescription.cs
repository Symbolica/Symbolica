using System;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Implementation.System;

internal sealed class InputDescription : IPersistentDescription
{
    public (long, IPersistentDescription) Seek(long offset, uint whence)
    {
        return (-1L, this);
    }

    public int Read(ISpace space, IMemory memory, Address address, int count)
    {
        return -1;
    }

    public Address ReadDirectory(ISpace space, IMemory memory, IStruct entry, Address address, int tell)
    {
        return Address.CreateNull(space.PointerSize);
    }

    public int GetStatus(ISpace space, IMemory memory, IStruct stat, Address address)
    {
        var type = Convert.ToInt32("0020000", 8);
        var mode = Convert.ToInt32("00444", 8);

        memory.Write(address, stat
            .Write(space, 3, type | mode)
            .Write(space, 8, 0L)
            .Expression);

        return 0;
    }
}
