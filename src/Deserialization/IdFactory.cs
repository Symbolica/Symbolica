using System;
using System.Collections.Generic;

namespace Symbolica.Deserialization;

internal sealed class IdFactory : IIdFactory
{
    private readonly Dictionary<IntPtr, ulong> _ids;
    private ulong _id;

    public IdFactory()
    {
        _ids = new Dictionary<IntPtr, ulong>();
        _id = 0UL;
    }

    public ulong GetOrCreate(IntPtr handle)
    {
        return _ids.TryGetValue(handle, out var id)
            ? id
            : Create(handle);
    }

    private ulong Create(IntPtr handle)
    {
        _ids.Add(handle, ++_id);
        return _id;
    }
}
