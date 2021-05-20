#ifndef DIS_STRUCT_ELEMENT_DTO_H
#define DIS_STRUCT_ELEMENT_DTO_H

#include "../Serializable.h"

namespace {
    struct StructElementDto : Serializable {
        uint64_t offset;
        OperandDto *operand;

        explicit StructElementDto(uint64_t offset, OperandDto *operand)
                : offset(offset),
                  operand(operand) {}

    protected:
        void SerializeProperties() const override {
            SERIALIZE(offset);
            SERIALIZE(operand);
        }
    };
}

#endif