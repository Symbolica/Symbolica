#ifndef DIS_ATTRIBUTES_DTO_H
#define DIS_ATTRIBUTES_DTO_H

#include "../Serializable.h"

namespace {
    struct AttributesDto : Serializable {
        bool isSignExtended;

        explicit AttributesDto(bool isSignExtended)
                : isSignExtended(isSignExtended) {}

    protected:
        void SerializeProperties() const override {
            SERIALIZE(isSignExtended);
        }
    };
}

#endif
