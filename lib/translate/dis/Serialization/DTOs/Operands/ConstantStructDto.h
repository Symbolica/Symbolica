#ifndef DIS_CONSTANT_STRUCT_DTO_H
#define DIS_CONSTANT_STRUCT_DTO_H

#include "../StructElementDto.h"
#include "OperandDto.h"

namespace {
    struct ConstantStructDto : OperandDto {
        uint64_t size;
        std::vector<StructElementDto *> elements;

        explicit ConstantStructDto(const char *type, ConstantStruct *constant, DataLayout *dataLayout)
                : OperandDto(type),
                  size(dataLayout->getTypeStoreSizeInBits(constant->getType())) {
            auto *structLayout = dataLayout->getStructLayout(constant->getType());
            for (unsigned i = 0; i < constant->getNumOperands(); ++i) {
                elements.push_back(new StructElementDto(structLayout->getElementOffsetInBits(i), map(constant->getOperand(i), dataLayout)));
            }
        }

    protected:
        void SerializeProperties() const override {
            OperandDto::SerializeProperties();
            SERIALIZE(size);
            SERIALIZE(elements);
        }
    };
}

#endif
