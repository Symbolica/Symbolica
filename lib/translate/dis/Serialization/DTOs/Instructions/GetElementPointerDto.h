#ifndef DIS_GET_ELEMENT_POINTER_DTO_H
#define DIS_GET_ELEMENT_POINTER_DTO_H

#include "../OffsetDto.h"
#include "InstructionDto.h"

namespace {
    struct GetElementPointerDto : InstructionDto {
        std::vector<uint64_t> constantOffsets;
        std::vector<OffsetDto *> offsets;

        explicit GetElementPointerDto(const char *type, GetElementPtrInst *instruction, DataLayout *dataLayout)
                : InstructionDto(type, instruction, dataLayout) {
            auto *indexedType = instruction->getPointerOperandType();
            for (auto &index : instruction->indices()) {
                if (auto *structType = dyn_cast<StructType>(indexedType)) {
                    auto *structLayout = dataLayout->getStructLayout(structType);
                    auto indexConstant = (unsigned) cast<ConstantInt>(index.get())->getZExtValue();
                    indexedType = structType->getTypeAtIndex(indexConstant);
                    constantOffsets.push_back(structLayout->getElementOffset(indexConstant));
                } else {
                    if (auto *arrayType = dyn_cast<ArrayType>(indexedType)) {
                        indexedType = arrayType->getElementType();
                    } else if (auto *vectorType = dyn_cast<VectorType>(indexedType)) {
                        indexedType = vectorType->getElementType();
                    } else {
                        indexedType = cast<PointerType>(indexedType)->getElementType();
                    }
                    offsets.push_back(new OffsetDto(index.getOperandNo(), dataLayout->getTypeStoreSize(indexedType)));
                }
            }
        }

    protected:
        void SerializeProperties() const override {
            InstructionDto::SerializeProperties();
            SERIALIZE(constantOffsets);
            SERIALIZE(offsets);
        }
    };
}

#endif
