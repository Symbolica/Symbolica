#ifndef DIS_CALL_DTO_H
#define DIS_CALL_DTO_H

#include "../AttributesDto.h"
#include "InstructionDto.h"

namespace {
    struct CallDto : InstructionDto {
        uint64_t size;
        AttributesDto *attributes;
        std::vector<AttributesDto *> parameterAttributes;

        explicit CallDto(const char *type, CallBase *instruction, DataLayout *dataLayout)
                : InstructionDto(type, instruction, dataLayout) {
            auto *instructionType = instruction->getType();
            size = instructionType->isSized() ? dataLayout->getTypeStoreSizeInBits(instructionType) : 0;
            attributes = new AttributesDto(instruction->hasRetAttr(Attribute::SExt));
            for (auto &argument : instruction->args()) {
                parameterAttributes.push_back(new AttributesDto(instruction->paramHasAttr(argument.getOperandNo(), Attribute::SExt)));
            }
        }

    protected:
        void SerializeProperties() const override {
            InstructionDto::SerializeProperties();
            SERIALIZE(size);
            SERIALIZE(attributes);
            SERIALIZE(parameterAttributes);
        }
    };
}

#endif