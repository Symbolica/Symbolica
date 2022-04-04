using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IStructType
{
    Bits Size { get; }

    Offset GetOffset(int index);
    IStruct CreateStruct(IExpression<IType> expression);
}
