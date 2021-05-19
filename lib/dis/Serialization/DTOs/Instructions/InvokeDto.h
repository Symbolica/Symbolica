#ifndef DIS_INVOKE_DTO_H
#define DIS_INVOKE_DTO_H

#include "CallDto.h"

namespace {
    struct InvokeDto : InstructionDto {
        CallDto *call;
        uint64_t successorId;

        explicit InvokeDto(const char *type, InvokeInst *instruction, DataLayout *dataLayout)
                : InstructionDto(type, instruction, dataLayout),
                  call(new CallDto(type, instruction, dataLayout)),
                  successorId(getId(instruction->getNormalDest())) {}

    protected:
        void SerializeProperties() const override {
            InstructionDto::SerializeProperties();
            SERIALIZE(call);
            SERIALIZE(successorId);
        }
    };
}

#endif