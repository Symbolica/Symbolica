FROM alpine:3.14.2 as base
ENV HOME /home/me
ARG llvmParallelJobs
RUN llvmParallelJobs="${llvmParallelJobs:-$(nproc)}" && \
    apk add --no-cache binutils && \
    apk add --no-cache --virtual build-deps build-base cmake git ninja python3 && \
    mkdir -p /tmp/llvm/build && \
    cd /tmp/llvm && \
    git clone --depth 1 --branch llvmorg-12.0.1 https://github.com/llvm/llvm-project.git && \
    cd build && \
    cmake -G Ninja \
        -Wno-dev \
        -DCLANG_BUILD_EXAMPLES=OFF \
        -DCLANG_INCLUDE_DOCS=OFF \
        -DCLANG_INCLUDE_TESTS=OFF \
        -DCLANG_PLUGIN_SUPPORT=OFF \
        -DCLANG_VENDOR=Alpine \
        -DCMAKE_BUILD_TYPE=MinSizeRel \
        -DCMAKE_VERBOSE_MAKEFILE=OFF \
        -DLIBCLANG_BUILD_STATIC=OFF \
        -DLLVM_APPEND_VC_REV=OFF \
        -DLLVM_BUILD_DOCS=OFF \
        -DLLVM_BUILD_EXAMPLES=OFF \
        -DLLVM_BUILD_EXTERNAL_COMPILER_RT=ON \
        -DLLVM_BUILD_LLVM_DYLIB=ON \
        -DLLVM_BUILD_TESTS=OFF \
        -DLLVM_ENABLE_ASSERTIONS=OFF \
        -DLLVM_ENABLE_EH=ON \
        -DLLVM_ENABLE_FFI=OFF \
        -DLLVM_ENABLE_LIBCXX=OFF \
        -DLLVM_ENABLE_PIC=ON \
        -DLLVM_ENABLE_PROJECTS=clang \
        -DLLVM_ENABLE_RTTI=ON \
        -DLLVM_ENABLE_SPHINX=OFF \
        -DLLVM_ENABLE_TERMINFO=ON \
        -DLLVM_ENABLE_ZLIB=ON \
        -DLLVM_INCLUDE_EXAMPLES=OFF \
        -DLLVM_LINK_LLVM_DYLIB=ON \
        -DLLVM_PARALLEL_LINK_JOBS=${llvmParallelJobs} \
        -DLLVM_TARGETS_TO_BUILD=Native \
        /tmp/llvm/llvm-project/llvm && \
    ninja -j ${llvmParallelJobs} install-llvm-link install-clang && \
    mv /usr/local/bin/llvm-link /usr/local/bin/llvm-link-12 && \
    cd / && \
    rm -rf /tmp && \
    apk --purge del build-deps

FROM base as musl-builder
WORKDIR $HOME/.symbolica/build
COPY . .
RUN apk add --no-cache git make && \
    ./musl.sh

FROM base
LABEL maintainer "Symbolica <dev@symbolica.dev>"

# TODO: Only copy across the built musl artifacts
COPY --from=musl-builder $HOME/.symbolica/build $HOME/.symbolica/build

WORKDIR /code
ENTRYPOINT [ "./symbolica.sh" ]
