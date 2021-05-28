#ifndef DIS_OPERAND_MAPPER_H
#define DIS_OPERAND_MAPPER_H

#include "DTOs/Operands/ArgumentDto.h"
#include "DTOs/Operands/BlockLabelDto.h"
#include "DTOs/Operands/ConstantExpressionDto.h"
#include "DTOs/Operands/ConstantScalarDto.h"
#include "DTOs/Operands/ConstantSequenceDto.h"
#include "DTOs/Operands/ConstantStructDto.h"
#include "DTOs/Operands/ConstantZeroDto.h"
#include "DTOs/Operands/GlobalAliasDto.h"
#include "DTOs/Operands/GlobalValueDto.h"
#include "DTOs/Operands/UndefinedDto.h"
#include "DTOs/Operands/VariableDto.h"

namespace {
    static OperandDto *map(Value *operand, DataLayout *dataLayout) {
        auto valueId = operand->getValueID();

        switch (valueId) {
            case Value::FunctionVal:
                return new GlobalValueDto("Function", cast<Function>(operand));
            case Value::GlobalAliasVal:
                return new GlobalAliasDto("GlobalAlias", cast<GlobalAlias>(operand), dataLayout);
            case Value::GlobalIFuncVal:
                return new OperandDto("IndirectFunction");
            case Value::GlobalVariableVal:
                return new GlobalValueDto("GlobalVariable", cast<GlobalVariable>(operand));
            case Value::BlockAddressVal:
                return new BlockLabelDto("BlockAddress", cast<BlockAddress>(operand));
            case Value::ConstantExprVal:
                return new ConstantExpressionDto("ConstantExpression", cast<ConstantExpr>(operand), dataLayout);
            case Value::ConstantArrayVal:
                return new ConstantSequenceDto("ConstantArray", cast<ConstantArray>(operand), dataLayout);
            case Value::ConstantStructVal:
                return new ConstantStructDto("ConstantStruct", cast<ConstantStruct>(operand), dataLayout);
            case Value::ConstantVectorVal:
                return new ConstantSequenceDto("ConstantVector", cast<ConstantVector>(operand), dataLayout);
            case Value::UndefValueVal:
                return new UndefinedDto("Undefined", cast<UndefValue>(operand), dataLayout);
            case Value::ConstantAggregateZeroVal:
                return new ConstantZeroDto("ConstantZero", cast<ConstantAggregateZero>(operand), dataLayout);
            case Value::ConstantDataArrayVal:
                return new ConstantSequenceDto("ConstantDataArray", cast<ConstantDataArray>(operand), dataLayout);
            case Value::ConstantDataVectorVal:
                return new ConstantSequenceDto("ConstantDataVector", cast<ConstantDataVector>(operand), dataLayout);
            case Value::ConstantIntVal:
                return new ConstantScalarDto("ConstantInteger", cast<ConstantInt>(operand));
            case Value::ConstantFPVal:
                return new ConstantScalarDto("ConstantFloat", cast<ConstantFP>(operand));
            case Value::ConstantPointerNullVal:
                return new OperandDto("ConstantNull");
            case Value::ConstantTokenNoneVal:
                return new OperandDto("ConstantEmptyToken");
            case Value::ArgumentVal:
                return new ArgumentDto("Argument", cast<Argument>(operand));
            case Value::BasicBlockVal:
                return new BlockLabelDto("BasicBlock", cast<BasicBlock>(operand));
            case Value::MetadataAsValueVal:
                return new OperandDto("Metadata");
            case Value::InlineAsmVal:
                return new OperandDto("Assembly");
            default:
                return valueId > Value::InstructionVal
                    ? new VariableDto("Variable", cast<Instruction>(operand))
                    : new OperandDto("Unknown");
        }
    }
}

#endif
