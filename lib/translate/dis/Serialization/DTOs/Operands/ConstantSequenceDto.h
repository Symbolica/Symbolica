#ifndef DIS_CONSTANT_SEQUENCE_DTO_H
#define DIS_CONSTANT_SEQUENCE_DTO_H

#include "OperandDto.h"

namespace {
    struct ConstantSequenceDto : OperandDto {
        uint64_t size;
        std::vector<OperandDto *> elements;

        explicit ConstantSequenceDto(const char *type, ConstantAggregate *constant, DataLayout *dataLayout)
                : ConstantSequenceDto(type, cast<Constant>(constant), dataLayout) {
            for (auto &operand : constant->operands()) {
                elements.push_back(map(operand, dataLayout));
            }
        }

        explicit ConstantSequenceDto(const char *type, ConstantDataSequential *constant, DataLayout *dataLayout)
                : ConstantSequenceDto(type, cast<Constant>(constant), dataLayout) {
            for (unsigned i = 0; i < constant->getNumElements(); ++i) {
                elements.push_back(map(constant->getElementAsConstant(i), dataLayout));
            }
        }

    protected:
        void SerializeProperties() const override {
            OperandDto::SerializeProperties();
            SERIALIZE(size);
            SERIALIZE(elements);
        }

    private:
        explicit ConstantSequenceDto(const char *type, Constant *constant, DataLayout *dataLayout)
                : OperandDto(type),
                  size(dataLayout->getTypeStoreSizeInBits(constant->getType())) {}
    };
}

#endif
