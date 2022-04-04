using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.System;

internal interface IPersistentSystem
{
    (Address, IPersistentSystem) GetThreadAddress(ISpace space, IMemoryProxy memory);
    (int, IPersistentSystem) Open(string path);
    (int, IPersistentSystem) Duplicate(int descriptor);
    (int, IPersistentSystem) Close(int descriptor);
    (long, IPersistentSystem) Seek(int descriptor, long offset, uint whence);
    int Read(ISpace space, IMemory memory, int descriptor, Address address, int count);
    Address ReadDirectory(ISpace space, IMemory memory, Address address);
    int GetStatus(ISpace space, IMemory memory, int descriptor, Address address);
}
