using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.System
{
    internal interface IPersistentSystem
    {
        (IExpression, IPersistentSystem) GetThreadAddress(ISpace space, IMemoryProxy memory);
        (int, IPersistentSystem) Open(string path);
        (int, IPersistentSystem) Duplicate(int descriptor);
        (int, IPersistentSystem) Close(int descriptor);
        (long, IPersistentSystem) Seek(int descriptor, long offset, uint whence);
        int Read(ISpace space, IMemory memory, int descriptor, IExpression address, int count);
        IExpression ReadDirectory(ISpace space, IMemory memory, IExpression address);
        int GetStatus(ISpace space, IMemory memory, int descriptor, IExpression address);
    }
}
