#ifndef DIS_PARAMETER_DTO_H
#define DIS_PARAMETER_DTO_H

#include "../Serializable.h"

namespace {
    struct ParameterDto : Serializable {
        uint64_t size;

        explicit ParameterDto(uint64_t size)
                : size(size) {}

    protected:
        void SerializeProperties() const override {
            SERIALIZE(size);
        }
    };
}

#endif
