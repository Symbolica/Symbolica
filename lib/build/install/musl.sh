#!/usr/bin/env sh
set -e

git clone --depth 1 --branch v1.2.2 git://git.musl-libc.org/musl

cd musl
cp ~/.symbolica/build/install/atomic_arch.h arch/x86_64
rm -r src/fenv/x86_64 src/ldso src/math/x86_64 src/process/x86_64 src/string/x86_64
CODEGEN=1 NO_LLVM=1 CC=~/.symbolica/build/clang CFLAGS="-O0 -Xclang -disable-O0-optnone -fPIC -U__GNUC__" ./configure --disable-shared --prefix=$(eval echo ~/.symbolica/build/sysroot)
CODEGEN=1 make -j$(nproc)
CODEGEN=1 NO_LLVM=1 make install
ls ~/.symbolica
ls ~/.symbolica/build
ls ~/.symbolica/build/sysroot
ls ~/.symbolica/build/sysroot/include
ls ~/.symbolica/build/sysroot/lib
ls ~/.symbolica/build/sysroot/bin
mkdir -p ~/.symbolica/build/musl
find obj/src -name "*.lo.bc" ! -path "obj/src/thread/x86_64/*" ! -name setjmp.lo.bc ! -name longjmp.lo.bc ! -name sigsetjmp.lo.bc ! -name restore.lo.bc -print0 | xargs -0 ~/.symbolica/build/llvm-link -o ~/.symbolica/build/musl/libc.bc
cd ..
