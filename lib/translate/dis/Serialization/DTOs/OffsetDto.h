#ifndef DIS_OFFSET_DTO_H
#define DIS_OFFSET_DTO_H

#include "../Serializable.h"

namespace {
    struct OffsetDto : Serializable {
        unsigned operandNumber;
        uint64_t elementSize;

        explicit OffsetDto(unsigned operandNumber, uint64_t elementSize)
                : operandNumber(operandNumber),
                  elementSize(elementSize) {}

    protected:
        void SerializeProperties() const override {
            SERIALIZE(operandNumber);
            SERIALIZE(elementSize);
        }
    };
}

#endif
