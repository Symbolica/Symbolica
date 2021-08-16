#ifndef DIS_SERIALIZER_H
#define DIS_SERIALIZER_H

#include "DTOs/ModuleDto.h"

namespace {
    class Serializer {
    public:
        static void SerializeModule(LLVMContext &context, Module *module) {
            auto *dto = new ModuleDto(context, module, new DataLayout(module));
            dto->Serialize();
        }
    };
}

#endif
