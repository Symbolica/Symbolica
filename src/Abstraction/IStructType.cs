using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IStructType : IType
{
    IStruct CreateStruct(IExpression expression);
}
