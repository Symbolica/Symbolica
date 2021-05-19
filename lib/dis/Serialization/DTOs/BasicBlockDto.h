#ifndef DIS_BASIC_BLOCK_DTO_H
#define DIS_BASIC_BLOCK_DTO_H

#include "../InstructionMapper.h"

namespace {
    struct BasicBlockDto : Serializable {
        uint64_t id;
        std::vector<InstructionDto *> instructions;

        explicit BasicBlockDto(BasicBlock *basicBlock, DataLayout *dataLayout)
                : id(getId(basicBlock)) {
            for (auto &instruction : *basicBlock) {
                instructions.push_back(map(&instruction, dataLayout));
            }
        }

    protected:
        void SerializeProperties() const override {
            SERIALIZE(id);
            SERIALIZE(instructions);
        }
    };
}

#endif