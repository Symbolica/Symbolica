#ifndef DIS_GLOBAL_ALIAS_DTO_H
#define DIS_GLOBAL_ALIAS_DTO_H

#include "OperandDto.h"

namespace {
    struct GlobalAliasDto : OperandDto {
        OperandDto *operand;

        explicit GlobalAliasDto(const char *type, GlobalAlias *globalAlias, DataLayout *dataLayout)
                : OperandDto(type),
                  operand(map(globalAlias->getAliasee(), dataLayout)) {}

    protected:
        void SerializeProperties() const override {
            OperandDto::SerializeProperties();
            SERIALIZE(operand);
        }
    };
}

#endif