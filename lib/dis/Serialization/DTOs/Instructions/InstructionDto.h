#ifndef DIS_INSTRUCTION_DTO_H
#define DIS_INSTRUCTION_DTO_H

#include "../../OperandMapper.h"

namespace {
    struct InstructionDto : PolymorphicSerializable {
        uint64_t id;
        std::vector<OperandDto *> operands;

        explicit InstructionDto(const char *type, Instruction *instruction, DataLayout *dataLayout)
                : PolymorphicSerializable(type),
                  id(getId(instruction)) {
            for (auto &operand : instruction->operands()) {
                operands.push_back(map(operand, dataLayout));
            }
        }

    protected:
        void SerializeProperties() const override {
            PolymorphicSerializable::SerializeProperties();
            SERIALIZE(id);
            SERIALIZE(operands);
        }
    };
}

#endif