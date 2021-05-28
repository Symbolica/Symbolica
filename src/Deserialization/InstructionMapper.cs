using System;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Instructions;
using Symbolica.Representation.Instructions;

namespace Symbolica.Deserialization
{
    internal static class InstructionMapper
    {
        public static IInstruction Map(InstructionDto dto)
        {
            var id = (InstructionId) dto.Id;
            var operands = dto.Operands.Convert();

            return dto.Type switch
            {
                "Return" => new Return(id, operands),
                "Branch" => new Branch(id, operands),
                "Switch" => new Switch(id, operands),
                "IndirectBranch" => new IndirectBranch(id, operands),
                "Invoke" => dto.As<InvokeDto>().To(id, operands),
                "Resume" => new Unsupported(id, dto.Type),
                "Unreachable" => new Unsupported(id, dto.Type),
                "CleanupReturn" => new Unsupported(id, dto.Type),
                "CatchReturn" => new Unsupported(id, dto.Type),
                "CatchSwitch" => new Unsupported(id, dto.Type),
                "FloatNegate" => new FloatNegate(id, operands),
                "Add" => new Add(id, operands),
                "FloatAdd" => new FloatAdd(id, operands),
                "Subtract" => new Subtract(id, operands),
                "FloatSubtract" => new FloatSubtract(id, operands),
                "Multiply" => new Multiply(id, operands),
                "FloatMultiply" => new FloatMultiply(id, operands),
                "UnsignedDivide" => new UnsignedDivide(id, operands),
                "SignedDivide" => new SignedDivide(id, operands),
                "FloatDivide" => new FloatDivide(id, operands),
                "UnsignedRemainder" => new UnsignedRemainder(id, operands),
                "SignedRemainder" => new SignedRemainder(id, operands),
                "FloatRemainder" => new FloatRemainder(id, operands),
                "ShiftLeft" => new ShiftLeft(id, operands),
                "LogicalShiftRight" => new LogicalShiftRight(id, operands),
                "ArithmeticShiftRight" => new ArithmeticShiftRight(id, operands),
                "And" => new And(id, operands),
                "Or" => new Or(id, operands),
                "Xor" => new Xor(id, operands),
                "Allocate" => dto.As<AllocateDto>().To(id, operands),
                "Load" => dto.As<LoadDto>().To(id, operands),
                "Store" => new Store(id, operands),
                "GetElementPointer" => dto.As<GetElementPointerDto>().To(id, operands),
                "Fence" => throw new Exception("Use loweratomic pass to lower to non-atomic form."),
                "AtomicCompareExchange" => throw new Exception("Use loweratomic pass to lower to non-atomic form."),
                "AtomicReadModifyWrite" => throw new Exception("Use loweratomic pass to lower to non-atomic form."),
                "Truncate" => dto.As<CastDto>().ToTruncate(id, operands),
                "ZeroExtend" => dto.As<CastDto>().ToZeroExtend(id, operands),
                "SignExtend" => dto.As<CastDto>().ToSignExtend(id, operands),
                "FloatToUnsigned" => dto.As<CastDto>().ToFloatToUnsigned(id, operands),
                "FloatToSigned" => dto.As<CastDto>().ToFloatToSigned(id, operands),
                "UnsignedToFloat" => dto.As<CastDto>().ToUnsignedToFloat(id, operands),
                "SignedToFloat" => dto.As<CastDto>().ToSignedToFloat(id, operands),
                "FloatTruncate" => dto.As<CastDto>().ToFloatTruncate(id, operands),
                "FloatExtend" => dto.As<CastDto>().ToFloatExtend(id, operands),
                "PointerToInteger" => dto.As<CastDto>().ToPointerToInteger(id, operands),
                "IntegerToPointer" => dto.As<CastDto>().ToIntegerToPointer(id, operands),
                "BitCast" => new BitCast(id, operands),
                "AddressSpaceCast" => new Unsupported(id, dto.Type),
                "CleanupPad" => new Unsupported(id, dto.Type),
                "CatchPad" => new Unsupported(id, dto.Type),
                "Compare" => dto.As<CompareDto>().To(id, operands),
                "FloatCompare" => dto.As<CompareDto>().To(id, operands),
                "Phi" => dto.As<PhiDto>().To(id, operands),
                "Call" => dto.As<CallDto>().To(id, operands),
                "Select" => new Select(id, operands),
                "UserOp1" => throw new Exception("Should only be used internally in passes."),
                "UserOp2" => throw new Exception("Should only be used internally in passes."),
                "VariableArgument" => throw new Exception("Should be lowered by front-end."),
                "ExtractElement" => dto.As<VectorDto>().ToExtractElement(id, operands),
                "InsertElement" => dto.As<VectorDto>().ToInsertElement(id, operands),
                "ShuffleVector" => throw new Exception("Use scalarizer pass to convert to scalar operations."),
                "ExtractValue" => dto.As<AggregateDto>().ToExtractValue(id, operands),
                "InsertValue" => dto.As<AggregateDto>().ToInsertValue(id, operands),
                "LandingPad" => new Unsupported(id, dto.Type),
                "Unknown" => throw new Exception("Instruction type is unknown."),
                _ => throw new Exception($"Instruction type {dto.Type} is unknown.")
            };
        }
    }
}
