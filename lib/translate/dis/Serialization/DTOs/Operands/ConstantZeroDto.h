#ifndef DIS_CONSTANT_ZERO_DTO_H
#define DIS_CONSTANT_ZERO_DTO_H

#include "OperandDto.h"

namespace {
    struct ConstantZeroDto : OperandDto {
        uint64_t size;

        explicit ConstantZeroDto(const char *type, ConstantAggregateZero *constant, DataLayout *dataLayout)
                : OperandDto(type),
                  size(dataLayout->getTypeStoreSizeInBits(constant->getType())) {}

    protected:
        void SerializeProperties() const override {
            OperandDto::SerializeProperties();
            SERIALIZE(size);
        }
    };
}

#endif
