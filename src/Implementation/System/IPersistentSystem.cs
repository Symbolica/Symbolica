using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.System;

internal interface IPersistentSystem
{
    (IExpression<IType>, IPersistentSystem) GetThreadAddress(ISpace space, IMemoryProxy memory);
    (int, IPersistentSystem) Open(string path);
    (int, IPersistentSystem) Duplicate(int descriptor);
    (int, IPersistentSystem) Close(int descriptor);
    (long, IPersistentSystem) Seek(int descriptor, long offset, uint whence);
    int Read(ISpace space, IMemory memory, int descriptor, IExpression<IType> address, int count);
    IExpression<IType> ReadDirectory(ISpace space, IMemory memory, IExpression<IType> address);
    int GetStatus(ISpace space, IMemory memory, int descriptor, IExpression<IType> address);
}
