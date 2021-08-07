#ifndef DIS_CAST_DTO_H
#define DIS_CAST_DTO_H

#include "InstructionDto.h"

namespace {
    struct CastDto : InstructionDto {
        uint64_t size;

        explicit CastDto(const char *type, Instruction *instruction, DataLayout *dataLayout)
                : InstructionDto(type, instruction, dataLayout),
                  size(dataLayout->getTypeSizeInBits(instruction->getType())) {}

    protected:
        void SerializeProperties() const override {
            InstructionDto::SerializeProperties();
            SERIALIZE(size);
        }
    };
}

#endif
