using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface ISystem
{
    IExpression<IType> GetThreadAddress();
    int Open(string path);
    int Duplicate(int descriptor);
    int Close(int descriptor);
    long Seek(int descriptor, long offset, uint whence);
    int Read(int descriptor, IExpression<IType> address, int count);
    IExpression<IType> ReadDirectory(IExpression<IType> address);
    int GetStatus(int descriptor, IExpression<IType> address);
}
