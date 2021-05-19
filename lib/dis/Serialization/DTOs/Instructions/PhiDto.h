#ifndef DIS_PHI_DTO_H
#define DIS_PHI_DTO_H

#include "InstructionDto.h"

namespace {
    struct PhiDto : InstructionDto {
        std::vector<uint64_t> predecessorIds;

        explicit PhiDto(const char *type, PHINode *instruction, DataLayout *dataLayout)
                : InstructionDto(type, instruction, dataLayout) {
            for (auto &basicBlock : instruction->blocks()) {
                predecessorIds.push_back(getId(basicBlock));
            }
        }

    protected:
        void SerializeProperties() const override {
            InstructionDto::SerializeProperties();
            SERIALIZE(predecessorIds);
        }
    };
}

#endif