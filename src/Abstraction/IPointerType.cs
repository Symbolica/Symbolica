using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IPointerType : IType
{
    IType ElementType { get; }
    IType Deferefence(Bytes allocatedSize);
}
