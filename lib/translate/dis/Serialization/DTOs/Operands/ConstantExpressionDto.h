#ifndef DIS_CONSTANT_EXPRESSION_DTO_H
#define DIS_CONSTANT_EXPRESSION_DTO_H

#include "OperandDto.h"

namespace {
    struct ConstantExpressionDto : OperandDto {
        InstructionDto *instruction;

        explicit ConstantExpressionDto(const char *type, ConstantExpr *constant, DataLayout *dataLayout)
                : OperandDto(type),
                  instruction(map(constant->getAsInstruction(), dataLayout)) {}

    protected:
        void SerializeProperties() const override {
            OperandDto::SerializeProperties();
            SERIALIZE(instruction);
        }
    };
}

#endif
