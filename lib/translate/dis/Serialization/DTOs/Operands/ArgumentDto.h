#ifndef DIS_ARGUMENT_DTO_H
#define DIS_ARGUMENT_DTO_H

#include "OperandDto.h"

namespace {
    struct ArgumentDto : OperandDto {
        unsigned index;

        explicit ArgumentDto(const char *type, Argument *argument)
                : OperandDto(type),
                  index(argument->getArgNo()) {}

    protected:
        void SerializeProperties() const override {
            OperandDto::SerializeProperties();
            SERIALIZE(index);
        }
    };
}

#endif
