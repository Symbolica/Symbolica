#ifndef DIS_VARIABLE_DTO_H
#define DIS_VARIABLE_DTO_H

#include "OperandDto.h"

namespace {
    struct VariableDto : OperandDto {
        uint64_t instructionId;

        explicit VariableDto(const char *type, Instruction *instruction)
                : OperandDto(type),
                  instructionId(getId(instruction)) {}

    protected:
        void SerializeProperties() const override {
            OperandDto::SerializeProperties();
            SERIALIZE(instructionId);
        }
    };
}

#endif
