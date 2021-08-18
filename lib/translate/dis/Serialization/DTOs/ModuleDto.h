#ifndef DIS_MODULE_DTO_H
#define DIS_MODULE_DTO_H

#include "../FunctionMapper.h"
#include "GlobalDto.h"
#include "StructTypeDto.h"

namespace {
    struct ModuleDto : Serializable {
        std::string target;
        unsigned pointerSize;
        StructTypeDto *directoryStreamType;
        StructTypeDto *directoryEntryType;
        StructTypeDto *jumpBufferType;
        StructTypeDto *localeType;
        StructTypeDto *statType;
        StructTypeDto *threadType;
        StructTypeDto *vaListType;
        std::vector<FunctionDto *> functions;
        std::vector<GlobalDto *> globals;

        explicit ModuleDto(LLVMContext &context, Module *module, DataLayout *dataLayout)
                : target(module->getTargetTriple()),
                pointerSize(dataLayout->getPointerSizeInBits()),
                directoryStreamType(getStructType(context, dataLayout, "struct.__dirstream")),
                directoryEntryType(getStructType(context, dataLayout, "struct.dirent")),
                jumpBufferType(getStructType(context, dataLayout, "struct.__jmp_buf_tag")),
                localeType(getStructType(context, dataLayout, "struct.__locale_struct")),
                statType(getStructType(context, dataLayout, "struct.stat")),
                threadType(getStructType(context, dataLayout, "struct.__pthread")),
                vaListType(getStructType(context, dataLayout, "struct.__va_list_tag")) {
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
            SERIALIZE(directoryStreamType);
            SERIALIZE(directoryEntryType);
            SERIALIZE(jumpBufferType);
            SERIALIZE(localeType);
            SERIALIZE(statType);
            SERIALIZE(threadType);
            SERIALIZE(vaListType);
            SERIALIZE(functions);
            SERIALIZE(globals);
        }

    private:
        static StructTypeDto *getStructType(LLVMContext &context, DataLayout *dataLayout, StringRef name) {
            auto *type = StructType::getTypeByName(context, name);
            return type ? new StructTypeDto(type, dataLayout) : nullptr;
        }
    };
}

#endif
