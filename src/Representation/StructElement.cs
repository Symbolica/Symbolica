using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    [Serializable]
    public sealed class StructElement
    {
        public StructElement(Bits offset, IOperand operand)
        {
            Offset = offset;
            Operand = operand;
        }

        public Bits Offset { get; }
        public IOperand Operand { get; }
    }
}
