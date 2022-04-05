using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public sealed class StructElement
{
    public StructElement(Size offset, IOperand operand)
    {
        Offset = offset;
        Operand = operand;
    }

    public Size Offset { get; }
    public IOperand Operand { get; }
}
