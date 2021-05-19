using System;
using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Operands;
using Symbolica.Representation.Instructions;

namespace Symbolica.Deserialization.DTOs.Instructions
{
    internal sealed record CompareDto(
            string Type, ulong Id, IEnumerable<OperandDto> Operands, string ComparisonType)
        : InstructionDto(Type, Id, Operands)
    {
        public IInstruction To(InstructionId id, IOperand[] operands)
        {
            return ComparisonType switch
            {
                "FloatFalse" => new FloatFalse(id),
                "FloatOrderedAndEqual" => new FloatOrderedAndEqual(id, operands),
                "FloatOrderedAndGreater" => new FloatOrderedAndGreater(id, operands),
                "FloatOrderedAndGreaterOrEqual" => new FloatOrderedAndGreaterOrEqual(id, operands),
                "FloatOrderedAndLess" => new FloatOrderedAndLess(id, operands),
                "FloatOrderedAndLessOrEqual" => new FloatOrderedAndLessOrEqual(id, operands),
                "FloatOrderedAndNotEqual" => new FloatOrderedAndNotEqual(id, operands),
                "FloatOrdered" => new FloatOrdered(id, operands),
                "FloatUnordered" => new FloatUnordered(id, operands),
                "FloatUnorderedOrEqual" => new FloatUnorderedOrEqual(id, operands),
                "FloatUnorderedOrGreater" => new FloatUnorderedOrGreater(id, operands),
                "FloatUnorderedOrGreaterOrEqual" => new FloatUnorderedOrGreaterOrEqual(id, operands),
                "FloatUnorderedOrLess" => new FloatUnorderedOrLess(id, operands),
                "FloatUnorderedOrLessOrEqual" => new FloatUnorderedOrLessOrEqual(id, operands),
                "FloatUnorderedOrNotEqual" => new FloatUnorderedOrNotEqual(id, operands),
                "FloatTrue" => new FloatTrue(id),
                "BadFloatComparison" => throw new Exception("Float comparison type is bad."),
                "Equal" => new Equal(id, operands),
                "NotEqual" => new NotEqual(id, operands),
                "UnsignedGreater" => new UnsignedGreater(id, operands),
                "UnsignedGreaterOrEqual" => new UnsignedGreaterOrEqual(id, operands),
                "UnsignedLess" => new UnsignedLess(id, operands),
                "UnsignedLessOrEqual" => new UnsignedLessOrEqual(id, operands),
                "SignedGreater" => new SignedGreater(id, operands),
                "SignedGreaterOrEqual" => new SignedGreaterOrEqual(id, operands),
                "SignedLess" => new SignedLess(id, operands),
                "SignedLessOrEqual" => new SignedLessOrEqual(id, operands),
                "BadComparison" => throw new Exception("Comparison type is bad."),
                "Unknown" => throw new Exception("Comparison type is unknown."),
                _ => throw new Exception($"Comparison type {ComparisonType} is unknown.")
            };
        }
    }
}