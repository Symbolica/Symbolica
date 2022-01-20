#!/usr/bin/env sh
set -e

mkdir gmp
wget -qO- https://gmplib.org/download/gmp/gmp-6.1.2.tar.xz | tar -xJf - -C gmp --strip-components=1

cd gmp
CODEGEN=1 NO_LLVM=1 CC=~/.symbolica/bin/cc CFLAGS="-O0 -Xclang -disable-O0-optnone -fPIC" ./configure --disable-shared --disable-assembly --prefix=$(eval echo ~/.symbolica)
CODEGEN=1 make
CODEGEN=1 NO_LLVM=1 make install
find -name "*.o.bc" -print0 | xargs -0 ~/.symbolica/bin/llvm-link -o ~/.symbolica/lib/libgmp.bc
cd ..
