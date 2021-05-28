#ifndef DIS_GLOBAL_VALUE_DTO_H
#define DIS_GLOBAL_VALUE_DTO_H

#include "OperandDto.h"

namespace {
    struct GlobalValueDto : OperandDto {
        uint64_t id;

        explicit GlobalValueDto(const char *type, GlobalValue *globalValue)
                : OperandDto(type),
                  id(getId(globalValue)) {}

    protected:
        void SerializeProperties() const override {
            OperandDto::SerializeProperties();
            SERIALIZE(id);
        }
    };
}

#endif
