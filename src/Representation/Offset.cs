using Symbolica.Expression;

namespace Symbolica.Representation
{
    public sealed class Offset
    {
        public Offset(int operandNumber, Bytes elementSize)
        {
            OperandNumber = operandNumber;
            ElementSize = elementSize;
        }

        public int OperandNumber { get; }
        public Bytes ElementSize { get; }
    }
}