#ifndef DIS_SERIALIZER_H
#define DIS_SERIALIZER_H

#include "DTOs/ModuleDto.h"

namespace {
    class Serializer {
    public:
        static void SerializeModule(Module *module) {
            auto *dto = new ModuleDto(module, new DataLayout(module));
            dto->Serialize();
        }
    };
}

#endif