#ifndef DIS_LOAD_DTO_H
#define DIS_LOAD_DTO_H

#include "InstructionDto.h"

namespace {
    struct LoadDto : InstructionDto {
        uint64_t size;

        explicit LoadDto(const char *type, LoadInst *instruction, DataLayout *dataLayout)
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
