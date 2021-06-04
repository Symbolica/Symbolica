#include <unordered_map>
#include <llvm/IR/Instructions.h>
#include <llvm/IR/Module.h>
#include <llvm/IRReader/IRReader.h>
#include <llvm/Support/SourceMgr.h>

using namespace llvm;

uint64_t getId(void *pointer) {
    static std::unordered_map<void *, uint64_t> ids = {{nullptr, 0}};
    return ids.insert(std::make_pair(pointer, ids.size())).first->second;
}

#include "Serialization/Serializer.h"

int main() {
    SMDiagnostic diagnostic;
    LLVMContext context;

    auto buffer = MemoryBuffer::getSTDIN();
    if (auto ec = buffer.getError()) {
        fprintf(stderr, "%s\n", ec.message().c_str());
        return 1;
    }

    auto module = parseIR(buffer.get()->getMemBufferRef(), diagnostic, context);
    if (!module) {
        fprintf(stderr, "%s\n", diagnostic.getMessage().str().c_str());
        return 1;
    }

    Serializer::SerializeModule(module.get());

    return 0;
}
