#ifndef DIS_FUNCTION_MAPPER_H
#define DIS_FUNCTION_MAPPER_H

#include "DTOs/Functions/DefinitionDto.h"

namespace {
    static FunctionDto *map(Function *function, DataLayout *dataLayout) {
        return function->isDeclaration()
            ? new FunctionDto("Declaration", function, dataLayout)
            : new DefinitionDto("Definition", function, dataLayout);
    }
}

#endif
