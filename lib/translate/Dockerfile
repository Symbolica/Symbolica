ARG llvmParallelJobs
FROM alpine:3.14.2
LABEL maintainer "Symbolica <dev@symbolica.dev>"

RUN llvmParallelJobs="${llvmParallelJobs:-$(nproc)}" && \
    apk add --no-cache binutils && \
    apk add --no-cache --virtual build-deps build-base cmake git ninja python3 && \
    cd /tmp && \
    git clone --depth 1 --branch llvmorg-12.0.1 https://github.com/llvm/llvm-project.git && \
    cmake -G Ninja \
        -Wno-dev \
        -S llvm-project/llvm \
        -B llvm-project/build \
        -DCLANG_VENDOR=Alpine \
        -DCMAKE_BUILD_TYPE=MinSizeRel \
        -DLLVM_INCLUDE_EXAMPLES=OFF \
        -DLLVM_INCLUDE_TESTS=OFF \
        -DLLVM_PARALLEL_LINK_JOBS=${llvmParallelJobs} \
        -DLLVM_TARGETS_TO_BUILD=Native && \
    ninja -C llvm-project/build -j ${llvmParallelJobs} install-llvm-extract install-opt && \
    mv /usr/local/bin/llvm-extract /usr/local/bin/llvm-extract-12 && \
    mv /usr/local/bin/opt /usr/local/bin/opt-12 && \
    rm -rf /tmp/* && \
    apk --purge del build-deps

COPY . /.symbolica/

WORKDIR /code
ENTRYPOINT [ "/.symbolica/translate" ]
