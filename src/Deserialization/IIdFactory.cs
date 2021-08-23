using System;

namespace Symbolica.Deserialization
{
    internal interface IIdFactory
    {
        ulong GetOrCreate(IntPtr handle);
    }
}
