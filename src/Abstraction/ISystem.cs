using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface ISystem
{
    IExpression GetThreadAddress(ISpace space, IMemory memory);
    int Open(string path);
    int Duplicate(int descriptor);
    int Close(int descriptor);
    long Seek(int descriptor, long offset, uint whence);
    int Read(ISpace space, IMemory memory, int descriptor, IExpression address, int count);
    IExpression ReadDirectory(ISpace space, IMemory memory, IExpression address);
    int GetStatus(ISpace space, IMemory memory, int descriptor, IExpression address);
}
