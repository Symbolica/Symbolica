#ifndef DIS_GLOBAL_DTO_H
#define DIS_GLOBAL_DTO_H

#include "../OperandMapper.h"

namespace {
    struct GlobalDto : Serializable {
        uint64_t id;
        uint64_t size;
        OperandDto *initializer;

        explicit GlobalDto(GlobalVariable *globalVariable, DataLayout *dataLayout)
                : id(getId(globalVariable)),
                  size(dataLayout->getTypeStoreSizeInBits(globalVariable->getType()->getElementType())),
                  initializer(globalVariable->hasInitializer() ? map(globalVariable->getInitializer(), dataLayout) : nullptr) {}

    protected:
        void SerializeProperties() const override {
            SERIALIZE(id);
            SERIALIZE(size);
            SERIALIZE(initializer);
        }
    };
}

#endif
