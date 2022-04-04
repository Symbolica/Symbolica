using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Abstraction;

public interface ISystem
{
    Address GetThreadAddress();
    int Open(string path);
    int Duplicate(int descriptor);
    int Close(int descriptor);
    long Seek(int descriptor, long offset, uint whence);
    int Read(int descriptor, Address address, int count);
    Address ReadDirectory(Address address);
    int GetStatus(int descriptor, Address address);
}
