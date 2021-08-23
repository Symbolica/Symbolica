using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Operands;

namespace Symbolica.Deserialization.DTOs.Operands
{
    internal sealed record ConstantScalarDto(
            string Type, uint Size, string Value)
        : OperandDto(Type)
    {
        public IOperand To()
        {
            return new ConstantInteger((Bits) Size, BigInteger.Parse(Value));
        }
    }
}
