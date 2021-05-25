#ifndef DIS_FUNCTION_DTO_H
#define DIS_FUNCTION_DTO_H

#include "../ParameterDto.h"
#include "../../PolymorphicSerializable.h"

namespace {
    struct FunctionDto : PolymorphicSerializable {
        uint64_t id;
        std::string name;
        std::vector<ParameterDto *> parameters;

        explicit FunctionDto(const char *type, Function *function, DataLayout *dataLayout)
                : PolymorphicSerializable(type),
                  id(getId(function)),
                  name(function->getName()) {
            for (auto &argument : function->args()) {
                parameters.push_back(new ParameterDto(dataLayout->getTypeStoreSizeInBits(argument.getType())));
            }
        }

    protected:
        void SerializeProperties() const override {
            PolymorphicSerializable::SerializeProperties();
            SERIALIZE(id);
            SERIALIZE(name);
            SERIALIZE(parameters);
        }
    };
}

#endif