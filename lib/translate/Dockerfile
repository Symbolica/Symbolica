FROM alpine:3.14.2 as base
LABEL maintainer "Symbolica <dev@symbolica.dev>"
ARG clangParallelJobs

RUN clangParallelJobs="${clangParallelJobs:-$(nproc)}" && \
    installPrefix="/usr/local" && \
    apk add --no-cache binutils && \
    apk add --no-cache --virtual build-deps build-base cmake git ninja python3 && \
    mkdir -p /tmp/clang/install && \
    cd /tmp/clang && \
    git clone --depth 1 --branch llvmorg-12.0.1 https://github.com/llvm/llvm-project.git && \
    mkdir build && \
    cd build && \
    cmake -G Ninja \
        -Wno-dev \
        -DCMAKE_INSTALL_PREFIX="${installPrefix}" \
        -DCLANG_VENDOR=Alpine \
        -DCMAKE_BUILD_TYPE=MinSizeRel \
        -DLLVM_TARGETS_TO_BUILD=Native \
        -DLLVM_PARALLEL_LINK_JOBS=${clangParallelJobs} \
        -DLIBCLANG_BUILD_STATIC=ON \
        -DLLVM_DEFAULT_TARGET_TRIPLE=x86_64-alpine-linux-musl \
        -DLLVM_HOST_TRIPLE=x86_64-alpine-linux-musl \
        -DLLVM_ENABLE_EH=ON \
        -DLLVM_ENABLE_FFI=NO \
        -DLLVM_ENABLE_LIBCXX=NO \
        -DLLVM_ENABLE_PIC=YES \
        -DLLVM_ENABLE_RTTI=YES \
        /tmp/clang/llvm-project/llvm && \
    ninja -j ${clangParallelJobs} install-opt install-llvm-extract && \
    mv "${installPrefix}"/bin/opt "${installPrefix}"/bin/opt-12 && \
    mv "${installPrefix}"/bin/llvm-extract "${installPrefix}"/bin/llvm-extract-12 && \
    cd / && \
    rm -rf /tmp && \
    apk del build-deps

COPY . /.symbolica/

WORKDIR /code
ENTRYPOINT [ "/.symbolica/translate" ]
