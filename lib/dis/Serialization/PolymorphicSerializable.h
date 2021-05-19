#ifndef DIS_POLYMORPHIC_SERIALIZABLE_H
#define DIS_POLYMORPHIC_SERIALIZABLE_H

#include "Serializable.h"

namespace {
    struct FunctionDto;
    static FunctionDto *map(Function *function, DataLayout *dataLayout);

    struct InstructionDto;
    static InstructionDto *map(Instruction *instruction, DataLayout *dataLayout);

    struct OperandDto;
    static OperandDto *map(Value *operand, DataLayout *dataLayout);

    struct PolymorphicSerializable : Serializable {
        const char *type;

        explicit PolymorphicSerializable(const char *type)
                : type(type) {}

    protected:
        void SerializeProperties() const override {
            SERIALIZE(type);
        }
    };
}

#endif