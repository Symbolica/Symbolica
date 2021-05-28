#ifndef DIS_CONSTANT_SCALAR_DTO_H
#define DIS_CONSTANT_SCALAR_DTO_H

#include "OperandDto.h"

namespace {
    struct ConstantScalarDto : OperandDto {
        unsigned size;
        std::string value;

        explicit ConstantScalarDto(const char *type, ConstantInt *constant)
                : ConstantScalarDto(type, constant->getValue()) {}

        explicit ConstantScalarDto(const char *type, ConstantFP *constant)
                : ConstantScalarDto(type, constant->getValueAPF().bitcastToAPInt()) {}

    protected:
        void SerializeProperties() const override {
            OperandDto::SerializeProperties();
            SERIALIZE(size);
            SERIALIZE(value);
        }

    private:
        explicit ConstantScalarDto(const char *type, const APInt& value)
                : OperandDto(type),
                  size(value.getBitWidth()),
                  value(value.toString(10, false)) {}
    };
}

#endif
