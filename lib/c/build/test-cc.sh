#!/usr/bin/env sh
set -e

mkdir .test
cd .test
printf "#include <stdio.h>\nint main()\n{\n    printf(\"Hello world\");\n    return 0;\n}\n" > test.c

CODEGEN=1 NO_LLVM=1 ~/.symbolica/bin/cc test.c -o test
./test
cd ..
rm -rf .test
