using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Implementation.System;

internal interface IPersistentDescription
{
    (long, IPersistentDescription) Seek(long offset, uint whence);
    int Read(ISpace space, IMemory memory, Address address, int count);
    Address ReadDirectory(ISpace space, IMemory memory, IStruct entry, Address address, int tell);
    int GetStatus(ISpace space, IMemory memory, IStruct stat, Address address);
}
