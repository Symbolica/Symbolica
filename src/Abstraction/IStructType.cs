using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IStructType
{
    Bits Size { get; }

    Bits GetOffset(int index);
    IStruct CreateStruct(IExpression expression);
}
