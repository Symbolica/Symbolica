#!/usr/bin/env sh
set -e

git clone --depth 1 --branch v1.2.2 git://git.musl-libc.org/musl

cd musl
mv ~/.symbolica/atomic_arch.h arch/x86_64
rm -r src/fenv/x86_64 src/ldso src/math/x86_64 src/process/x86_64 src/string/x86_64
CODEGEN=1 NO_LLVM=1 CC=~/.symbolica/bin/clang CFLAGS="-O0 -Xclang -disable-O0-optnone -fPIC -U__GNUC__" ./configure --disable-shared --enable-wrapper=clang --prefix=$(eval echo ~/.symbolica)
CODEGEN=1 make -j$(nproc)
CODEGEN=1 NO_LLVM=1 make install
find obj/src -name "*.lo.bc" ! -path "obj/src/thread/x86_64/*" ! -name setjmp.lo.bc ! -name longjmp.lo.bc ! -name sigsetjmp.lo.bc ! -name restore.lo.bc -print0 | xargs -0 ~/.symbolica/bin/llvm-link -o ~/.symbolica/lib/libc.bc
cd ..
