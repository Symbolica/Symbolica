#ifndef DIS_VECTOR_DTO_H
#define DIS_VECTOR_DTO_H

#include "InstructionDto.h"

namespace {
    struct VectorDto : InstructionDto {
        uint64_t elementSize;
        uint64_t elementCount;

        explicit VectorDto(const char *type, ExtractElementInst *instruction, DataLayout *dataLayout)
                : VectorDto(type, instruction, dataLayout, instruction->getVectorOperandType()) {}

        explicit VectorDto(const char *type, InsertElementInst *instruction, DataLayout *dataLayout)
                : VectorDto(type, instruction, dataLayout, instruction->getType()) {}

    protected:
        void SerializeProperties() const override {
            InstructionDto::SerializeProperties();
            SERIALIZE(elementSize);
            SERIALIZE(elementCount);
        }

    private:
        explicit VectorDto(const char *type, Instruction *instruction, DataLayout *dataLayout, VectorType *vectorType)
                : InstructionDto(type, instruction, dataLayout),
                  elementSize(dataLayout->getTypeStoreSizeInBits(vectorType->getElementType())),
                  elementCount(vectorType->getNumElements()) {}
    };
}

#endif
