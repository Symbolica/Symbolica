ARG BUILD_BASE_IMG_TAG
FROM symbolica/build:${BUILD_BASE_IMG_TAG}
LABEL maintainer "Symbolica <dev@symbolica.dev>"

RUN apk add --no-cache linux-headers && \
    cp -r /usr/include/* /home/me/.symbolica/build/sysroot/include/ && \
    apk --purge del linux-headers

COPY ./cxx-bootstrap $HOME/.symbolica/build/cxx-bootstrap

RUN apk add --no-cache --virtual build-deps git cmake ninja python3 && \
    cd /tmp && \
    git clone --depth 1 --branch llvmorg-12.0.1 https://github.com/llvm/llvm-project.git && \
    cd llvm-project \
    mkdir build && \
    CODEGEN=1 NO_LLVM=1 CC=~/.symbolica/build/cc CXX=~/.symbolica/build/cxx-bootstrap \
        cmake -G Ninja \
        -S runtimes \
        -B build \
        -DCMAKE_BUILD_TYPE=MinSizeRel \
        -DCMAKE_INSTALL_PREFIX="${HOME}/.symbolica/build/sysroot" \
        -DCOMPILER_RT_BUILD_CRT=ON \
        -DCOMPILER_RT_BUILD_LIBFUZZER=OFF \
        -DCOMPILER_RT_BUILD_MEMPROF=OFF \
        -DCOMPILER_RT_BUILD_SANITIZERS=OFF \
        -DCOMPILER_RT_BUILD_XRAY=OFF \
        -DCOMPILER_RT_USE_BUILTINS_LIBRARY=OFF \
        -DCOMPILER_RT_USE_LIBCXX=ON \
        -DLIBCXX_CXX_ABI=libcxxabi \
        -DLIBCXX_ENABLE_EXCEPTIONS=OFF \
        -DLIBCXX_ENABLE_EXPERIMENTAL_LIBRARY=OFF \
        -DLIBCXX_ENABLE_SHARED=OFF \
        -DLIBCXX_ENABLE_STATIC_ABI_LIBRARY=ON \
        -DLIBCXX_HAS_GCC_LIB=OFF \
        -DLIBCXX_HAS_GCC_S_LIB=OFF \
        -DLIBCXX_HAS_MUSL_LIBC=ON \
        -DLIBCXX_STATICALLY_LINK_ABI_IN_STATIC_LIBRARY=ON \
        -DLIBCXXABI_ENABLE_EXCEPTIONS=OFF \
        -DLIBCXXABI_ENABLE_SHARED=OFF \
        -DLIBCXXABI_ENABLE_STATIC_UNWINDER=ON \
        -DLIBCXXABI_USE_LLVM_UNWINDER=ON \
        -DLIBUNWIND_ENABLE_SHARED=OFF \
        -DLLVM_ENABLE_LIBCXX=ON \
        -DLLVM_ENABLE_RUNTIMES="libunwind;libcxxabi;libcxx;compiler-rt" \
        -DTARGET_TRIPLE="x86_64-alpine-linux-musl" && \
    CODEGEN=1 ninja -C build cxx cxxabi unwind compiler-rt && \
    # # TODO: Enable testing once it's working
    # # ninja -C build check-cxx check-cxxabi check-unwind && \
    ninja -C build install-cxx install-cxxabi install-unwind install-compiler-rt && \
    find build/libunwind -name "*.o.bc" ! -name "*.S.o.bc" -print0 | xargs -0 ~/.symbolica/build/llvm-link -o ~/.symbolica/build/sysroot/lib/libunwind.bc && \
    find build/libcxxabi -name "*.o.bc" -print0 | xargs -0 ~/.symbolica/build/llvm-link -o ~/.symbolica/build/sysroot/lib/libcxxabi.bc && \
    find build/libcxx -name "*.o.bc" -print0 | xargs -0 ~/.symbolica/build/llvm-link -o ~/.symbolica/build/sysroot/lib/libcxx.bc && \
    cd / && \
    rm -rf /tmp/llvm-project && \
    apk --purge del build-deps && \
    rm $HOME/.symbolica/build/cxx-bootstrap

COPY ./cxx $HOME/.symbolica/build/cxx

WORKDIR /code
ENTRYPOINT [ "./symbolica.sh" ]
