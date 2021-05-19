using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Operands;
using Symbolica.Expression;
using Symbolica.Representation.Instructions;

namespace Symbolica.Deserialization.DTOs.Instructions
{
    internal sealed record CastDto(
            string Type, ulong Id, IEnumerable<OperandDto> Operands, uint Size)
        : InstructionDto(Type, Id, Operands)
    {
        public IInstruction ToTruncate(InstructionId id, IOperand[] operands)
        {
            return new Truncate(id, operands, (Bits) Size);
        }

        public IInstruction ToZeroExtend(InstructionId id, IOperand[] operands)
        {
            return new ZeroExtend(id, operands, (Bits) Size);
        }

        public IInstruction ToSignExtend(InstructionId id, IOperand[] operands)
        {
            return new SignExtend(id, operands, (Bits) Size);
        }

        public IInstruction ToFloatToUnsigned(InstructionId id, IOperand[] operands)
        {
            return new FloatToUnsigned(id, operands, (Bits) Size);
        }

        public IInstruction ToFloatToSigned(InstructionId id, IOperand[] operands)
        {
            return new FloatToSigned(id, operands, (Bits) Size);
        }

        public IInstruction ToUnsignedToFloat(InstructionId id, IOperand[] operands)
        {
            return new UnsignedToFloat(id, operands, (Bits) Size);
        }

        public IInstruction ToSignedToFloat(InstructionId id, IOperand[] operands)
        {
            return new SignedToFloat(id, operands, (Bits) Size);
        }

        public IInstruction ToFloatTruncate(InstructionId id, IOperand[] operands)
        {
            return new FloatTruncate(id, operands, (Bits) Size);
        }

        public IInstruction ToFloatExtend(InstructionId id, IOperand[] operands)
        {
            return new FloatExtend(id, operands, (Bits) Size);
        }

        public IInstruction ToPointerToInteger(InstructionId id, IOperand[] operands)
        {
            return new PointerToInteger(id, operands, (Bits) Size);
        }

        public IInstruction ToIntegerToPointer(InstructionId id, IOperand[] operands)
        {
            return new IntegerToPointer(id, operands, (Bits) Size);
        }
    }
}