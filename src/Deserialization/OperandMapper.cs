using System;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Operands;
using Symbolica.Representation.Operands;

namespace Symbolica.Deserialization
{
    internal static class OperandMapper
    {
        public static IOperand Map(OperandDto dto)
        {
            return dto.Type switch
            {
                "Function" => dto.As<GlobalValueDto>().ToFunction(),
                "GlobalAlias" => dto.As<GlobalAliasDto>().To(),
                "IndirectFunction" => new Unsupported(dto.Type),
                "GlobalVariable" => dto.As<GlobalValueDto>().ToGlobalVariable(),
                "BlockAddress" => dto.As<BlockLabelDto>().To(),
                "ConstantExpression" => dto.As<ConstantExpressionDto>().To(),
                "ConstantArray" => dto.As<ConstantSequenceDto>().To(),
                "ConstantStruct" => dto.As<ConstantStructDto>().To(),
                "ConstantVector" => dto.As<ConstantSequenceDto>().To(),
                "Undefined" => dto.As<UndefinedDto>().To(),
                "ConstantZero" => dto.As<ConstantZeroDto>().To(),
                "ConstantDataArray" => dto.As<ConstantSequenceDto>().To(),
                "ConstantDataVector" => dto.As<ConstantSequenceDto>().To(),
                "ConstantInteger" => dto.As<ConstantScalarDto>().To(),
                "ConstantFloat" => dto.As<ConstantScalarDto>().To(),
                "ConstantNull" => new ConstantNull(),
                "ConstantEmptyToken" => new Unsupported(dto.Type),
                "Argument" => dto.As<ArgumentDto>().To(),
                "BasicBlock" => dto.As<BlockLabelDto>().To(),
                "Metadata" => new Unsupported(dto.Type),
                "Assembly" => new Unsupported(dto.Type),
                "Variable" => dto.As<VariableDto>().To(),
                "Unknown" => throw new Exception("Operand type is unknown."),
                _ => throw new Exception($"Operand type {dto.Type} is unknown.")
            };
        }
    }
}