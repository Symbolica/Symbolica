#ifndef DIS_OPERAND_DTO_H
#define DIS_OPERAND_DTO_H

#include "../../PolymorphicSerializable.h"

namespace {
    struct OperandDto : PolymorphicSerializable {
        explicit OperandDto(const char *type)
                : PolymorphicSerializable(type) {}

    protected:
        void SerializeProperties() const override {
            PolymorphicSerializable::SerializeProperties();
        }
    };
}

#endif