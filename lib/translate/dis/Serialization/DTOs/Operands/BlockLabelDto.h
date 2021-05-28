#ifndef DIS_BLOCK_LABEL_DTO_H
#define DIS_BLOCK_LABEL_DTO_H

#include "OperandDto.h"

namespace {
    struct BlockLabelDto : OperandDto {
        uint64_t basicBlockId;

        explicit BlockLabelDto(const char *type, BasicBlock *basicBlock)
                : OperandDto(type),
                  basicBlockId(getId(basicBlock)) {}

        explicit BlockLabelDto(const char *type, BlockAddress *blockAddress)
                : OperandDto(type),
                  basicBlockId(getId(blockAddress->getBasicBlock())) {}

    protected:
        void SerializeProperties() const override {
            OperandDto::SerializeProperties();
            SERIALIZE(basicBlockId);
        }
    };
}

#endif
