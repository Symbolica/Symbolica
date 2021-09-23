FROM alpine:3.14.2 as base
LABEL maintainer "Symbolica <dev@symbolica.dev>"
ARG llvmParallelJobs

RUN llvmParallelJobs="${llvmParallelJobs:-$(nproc)}" && \
    apk add --no-cache binutils && \
    apk add --no-cache --virtual build-deps build-base cmake git ninja python3 && \
    mkdir -p /tmp/build && \
    cd /tmp && \
    git clone --depth 1 --branch llvmorg-12.0.1 https://github.com/llvm/llvm-project.git && \
    cd build && \
    cmake -G Ninja \
        -Wno-dev \
        -DCLANG_VENDOR=Alpine \
        -DCMAKE_BUILD_TYPE=MinSizeRel \
        -DLLVM_INCLUDE_EXAMPLES=OFF \
        -DLLVM_INCLUDE_TESTS=OFF \
        -DLLVM_PARALLEL_LINK_JOBS=${llvmParallelJobs} \
        -DLLVM_TARGETS_TO_BUILD=Native \
        /tmp/llvm-project/llvm && \
    ninja -j ${llvmParallelJobs} install-opt install-llvm-extract && \
    mv /usr/local/bin/opt /usr/local/bin/opt-12 && \
    mv /usr/local/bin/llvm-extract /usr/local/bin/llvm-extract-12 && \
    cd / && \
    rm -rf /tmp && \
    apk --purge del build-deps

COPY . /.symbolica/

WORKDIR /code
ENTRYPOINT [ "/.symbolica/translate" ]
