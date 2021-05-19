#ifndef DIS_DEFINITION_DTO_H
#define DIS_DEFINITION_DTO_H

#include "../BasicBlockDto.h"
#include "FunctionDto.h"

namespace {
    struct DefinitionDto : FunctionDto {
        bool isVariadic;
        std::vector<BasicBlockDto *> basicBlocks;

        explicit DefinitionDto(const char *type, Function *function, DataLayout *dataLayout)
                : FunctionDto(type, function, dataLayout),
                  isVariadic(function->isVarArg()) {
            for (auto &basicBlock : *function) {
                basicBlocks.push_back(new BasicBlockDto(&basicBlock, dataLayout));
            }
        }

    protected:
        void SerializeProperties() const override {
            FunctionDto::SerializeProperties();
            SERIALIZE(isVariadic);
            SERIALIZE(basicBlocks);
        }
    };
}

#endif