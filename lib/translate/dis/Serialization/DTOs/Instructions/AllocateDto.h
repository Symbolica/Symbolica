#ifndef DIS_ALLOCATE_DTO_H
#define DIS_ALLOCATE_DTO_H

#include "InstructionDto.h"

namespace {
    struct AllocateDto : InstructionDto {
        uint64_t elementSize;

        explicit AllocateDto(const char *type, AllocaInst *instruction, DataLayout *dataLayout)
                : InstructionDto(type, instruction, dataLayout),
                  elementSize(dataLayout->getTypeAllocSizeInBits(instruction->getAllocatedType())) {}

    protected:
        void SerializeProperties() const override {
            InstructionDto::SerializeProperties();
            SERIALIZE(elementSize);
        }
    };
}

#endif
