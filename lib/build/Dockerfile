FROM alpine:3.14.2 as base
LABEL maintainer "Symbolica <dev@symbolica.dev>"
ARG clangParallelJobs
ENV HOME /home/me

RUN clangParallelJobs="${clangParallelJobs:-$(nproc)}" && \
    apk add --no-cache binutils && \
    apk add --no-cache --virtual build-deps build-base cmake git ninja python3 && \
    mkdir -p /tmp/clang/build && \
    cd /tmp/clang && \
    git clone --depth 1 --branch llvmorg-12.0.1 https://github.com/llvm/llvm-project.git && \
    cd build && \
    cmake -G Ninja \
        -Wno-dev \
        -DCLANG_VENDOR=Alpine \
        -DLLVM_ENABLE_PROJECTS=clang \
        -DCMAKE_BUILD_TYPE=Release \
        -DLLVM_TARGETS_TO_BUILD=Native \
        -DLLVM_PARALLEL_LINK_JOBS=${clangParallelJobs} \
        -DLLVM_ENABLE_EH=ON \
        -DLLVM_ENABLE_FFI=NO \
        -DLLVM_ENABLE_LIBCXX=NO \
        -DLLVM_ENABLE_PIC=YES \
        -DLLVM_ENABLE_RTTI=YES \
        /tmp/clang/llvm-project/llvm && \
    ninja -j ${clangParallelJobs} install-llvm-link install-clang && \
    mv /usr/local/bin/llvm-link /usr/local/bin/llvm-link-12 && \
    cd / && \
    rm -rf /tmp && \
    apk --purge del build-deps

COPY . $HOME/.symbolica/build

# Install findutils because the default busybox xargs doesnt handle large inputs well
# and it ends up batching the calls to llvm-link which results in us only producing
# a libc.bc file with the contents of the final invocation in it.
RUN apk add --no-cache --virtual musl-build-deps git make findutils && \
    cd $HOME/.symbolica/build/install &&  \
    ./musl.sh && \
    cd / && \
    rm -rf $HOME/.symbolica/build/install && \
    apk --purge del musl-build-deps

WORKDIR /code
ENTRYPOINT [ "./symbolica.sh" ]
