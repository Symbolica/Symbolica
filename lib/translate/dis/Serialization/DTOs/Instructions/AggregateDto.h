#ifndef DIS_AGGREGATE_DTO_H
#define DIS_AGGREGATE_DTO_H

#include "InstructionDto.h"

namespace {
    struct AggregateDto : InstructionDto {
        uint64_t size;
        std::vector<uint64_t> constantOffsets;

        explicit AggregateDto(const char *type, ExtractValueInst *instruction, DataLayout *dataLayout)
                : AggregateDto(type, instruction, dataLayout, instruction->getAggregateOperand(), instruction->getIndices()) {}

        explicit AggregateDto(const char *type, InsertValueInst *instruction, DataLayout *dataLayout)
                : AggregateDto(type, instruction, dataLayout, instruction->getAggregateOperand(), instruction->getIndices()) {}

    protected:
        void SerializeProperties() const override {
            InstructionDto::SerializeProperties();
            SERIALIZE(size);
            SERIALIZE(constantOffsets);
        }

    private:
        explicit AggregateDto(const char *type, Instruction *instruction, DataLayout *dataLayout, Value *aggregate, ArrayRef<unsigned> indices)
                : InstructionDto(type, instruction, dataLayout),
                  size(dataLayout->getTypeStoreSizeInBits(instruction->getType())) {
            auto *indexedType = aggregate->getType();
            for (auto index : indices) {
                if (auto *structType = dyn_cast<StructType>(indexedType)) {
                    auto *structLayout = dataLayout->getStructLayout(structType);
                    indexedType = structType->getTypeAtIndex(index);
                    constantOffsets.push_back(structLayout->getElementOffsetInBits(index));
                } else {
                    if (auto *sequentialType = dyn_cast<SequentialType>(indexedType)) {
                        indexedType = sequentialType->getTypeAtIndex(index);
                    }
                    constantOffsets.push_back(index * dataLayout->getTypeStoreSizeInBits(indexedType));
                }
            }
        }
    };
}

#endif
