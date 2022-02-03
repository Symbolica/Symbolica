using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface ISystem
{
    IExpression GetThreadAddress();
    int Open(IExpression path);
    int Duplicate(int descriptor);
    int Close(int descriptor);
    long Seek(int descriptor, long offset, uint whence);
    int Read(int descriptor, IExpression address, int count);
    IExpression ReadDirectory(IExpression address);
    int GetStatus(int descriptor, IExpression address);
}
