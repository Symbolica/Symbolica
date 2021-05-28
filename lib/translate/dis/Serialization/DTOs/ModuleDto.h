#ifndef DIS_MODULE_DTO_H
#define DIS_MODULE_DTO_H

#include "../FunctionMapper.h"
#include "GlobalDto.h"
#include "StructTypeDto.h"

namespace {
    struct ModuleDto : Serializable {
        std::string target;
        unsigned pointerSize;
        std::vector<StructTypeDto *> structTypes;
        std::vector<FunctionDto *> functions;
        std::vector<GlobalDto *> globals;

        explicit ModuleDto(Module *module, DataLayout *dataLayout)
                : target(module->getTargetTriple()),
                  pointerSize(dataLayout->getPointerSizeInBits()) {
            for (auto &structType : module->getIdentifiedStructTypes()) {
                structTypes.push_back(new StructTypeDto(structType, dataLayout));
            }
            for (auto &function : module->functions()) {
                functions.push_back(map(&function, dataLayout));
            }
            for (auto &globalVariable : module->globals()) {
                globals.push_back(new GlobalDto(&globalVariable, dataLayout));
            }
        }

    protected:
        void SerializeProperties() const override {
            SERIALIZE(target);
            SERIALIZE(pointerSize);
            SERIALIZE(structTypes);
            SERIALIZE(functions);
            SERIALIZE(globals);
        }
    };
}

#endif
