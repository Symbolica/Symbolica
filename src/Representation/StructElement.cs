using Symbolica.Abstraction;

namespace Symbolica.Representation;

public sealed class StructElement
{
    public StructElement(Expression.Offset offset, IOperand operand)
    {
        Offset = offset;
        Operand = operand;
    }

    public Expression.Offset Offset { get; }
    public IOperand Operand { get; }
}
