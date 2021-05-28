#include <fstream>
#include <unordered_map>
#include <llvm/IR/Instructions.h>
#include <llvm/IR/Module.h>
#include <llvm/IRReader/IRReader.h>
#include <llvm/Support/SourceMgr.h>

using namespace llvm;

std::ofstream &out() {
    static std::ofstream stream;
    return stream;
}

uint64_t getId(void *pointer) {
    static std::unordered_map<void *, uint64_t> ids = {{nullptr, 0}};
    return ids.insert(std::make_pair(pointer, ids.size())).first->second;
}

#include "Serialization/Serializer.h"

int main(int argc, char **argv) {
    SMDiagnostic diagnostic;
    LLVMContext context;
    auto module = parseIRFile(argv[1], diagnostic, context);
    if (!module) {
        fprintf(stderr, "%s\n", diagnostic.getMessage().str().c_str());
        return 1;
    }

    out().open(argv[2]);
    Serializer::SerializeModule(module.get());
    out().close();

    return 0;
}
