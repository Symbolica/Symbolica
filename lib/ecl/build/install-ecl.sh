#!/usr/bin/env sh
set -e

wget -qO- https://gmplib.org/download/gmp/gmp-6.1.2.tar.xz | tar -xJf - --one-top-level=gmp --strip-components=1
git clone --depth 1 --branch 20.4.24 https://gitlab.com/embeddable-common-lisp/ecl.git

cd gmp
CODEGEN=1 NO_LLVM=1 CC=~/.symbolica/bin/cc CFLAGS="-O0 -Xclang -disable-O0-optnone -fPIC" ./configure --disable-shared --disable-assembly --prefix=$(eval echo ~/.symbolica)
CODEGEN=1 make
CODEGEN=1 NO_LLVM=1 make install
find -name "*.o.bc" -print0 | xargs -0 ~/.symbolica/bin/llvm-link -o ~/.symbolica/lib/libgmp.bc
cd ..

cd ecl
CODEGEN=1 NO_LLVM=1 CC=~/.symbolica/bin/cc CFLAGS="-O0 -Xclang -disable-O0-optnone -fPIC -DNO_PROC_STAT" ./configure --disable-shared --disable-threads --enable-gmp=system --prefix=$(eval echo ~/.symbolica)
echo "#undef ECL_USE_MPROTECT" >> build/ecl/config.h
CODEGEN=1 make
CODEGEN=1 NO_LLVM=1 make install
find -name "*.o.bc" ! -path "./build/gc/*" ! -name all_symbols.o.bc ! -name cinit.o.bc ! -name unix64.o.bc ! -name win64.o.bc -print0 | xargs -0 ~/.symbolica/bin/llvm-link -o ~/.symbolica/lib/libecl.bc
cd ..