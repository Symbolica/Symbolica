#!/usr/bin/env sh
set -e

git clone --depth 1 --branch llvmorg-12.0.1 https://github.com/llvm/llvm-project.git

cd llvm-project
mkdir build
CODEGEN=1 NO_LLVM=1 CC=~/.symbolica/bin/cc CXX=~/.symbolica/bin/cc CXXFLAGS=-nostdinc++ \
    cmake -G Ninja \
    -Wno-dev \
    -S runtimes \
    -B build \
    -DCMAKE_BUILD_TYPE=Debug \
    -DCMAKE_INSTALL_PREFIX=~/.symbolica \
    -DLIBCXX_CXX_ABI=libcxxabi \
    -DLIBCXX_ENABLE_EXCEPTIONS=OFF \
    -DLIBCXX_ENABLE_EXPERIMENTAL_LIBRARY=OFF \
    -DLIBCXX_ENABLE_SHARED=OFF \
    -DLIBCXX_ENABLE_STATIC_ABI_LIBRARY=ON \
    -DLIBCXX_ENABLE_THREADS=OFF \
    -DLIBCXX_HAS_GCC_LIB=OFF \
    -DLIBCXX_HAS_GCC_S_LIB=OFF \
    -DLIBCXX_HAS_MUSL_LIBC=ON \
    -DLIBCXX_STATICALLY_LINK_ABI_IN_STATIC_LIBRARY=ON \
    -DLIBCXXABI_ENABLE_EXCEPTIONS=OFF \
    -DLIBCXXABI_ENABLE_SHARED=OFF \
    -DLIBCXXABI_ENABLE_STATIC_UNWINDER=ON \
    -DLIBCXXABI_USE_LLVM_UNWINDER=ON \
    -DLIBUNWIND_ENABLE_SHARED=OFF \
    -DLIBUNWIND_ENABLE_THREADS=OFF \
    -DLLVM_ENABLE_RUNTIMES="libcxx;libcxxabi;libunwind"
CODEGEN=1 ninja -C build cxx cxxabi unwind
CODEGEN=1 NO_LLVM=1 ninja -C build install-cxx install-cxxabi install-unwind
find build/libunwind -name "*.o.bc" ! -name "*.S.o.bc" -print0 | xargs -0 ~/.symbolica/bin/llvm-link -o ~/.symbolica/lib/libunwind.bc
find build/libcxxabi -name "*.o.bc" -print0 | xargs -0 ~/.symbolica/bin/llvm-link -o ~/.symbolica/lib/libcxxabi.bc
find build/libcxx -name "*.o.bc" -print0 | xargs -0 ~/.symbolica/bin/llvm-link -o ~/.symbolica/lib/libcxx.bc
cd ..
