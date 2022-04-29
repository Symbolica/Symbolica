using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IArrayType : IType
{
    IType ElementType { get; }

    IType Resize(Bytes allocatedSize);
}
