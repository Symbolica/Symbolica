#ifndef DIS_UNDEFINED_DTO_H
#define DIS_UNDEFINED_DTO_H

#include "OperandDto.h"

namespace {
    struct UndefinedDto : OperandDto {
        uint64_t size;

        explicit UndefinedDto(const char *type, UndefValue *constant, DataLayout *dataLayout)
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