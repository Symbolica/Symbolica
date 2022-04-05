using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IStructType
{
    Size Size { get; }

    Size GetOffset(int index);
    IStruct CreateStruct(IExpression expression);
}
