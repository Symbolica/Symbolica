ARG llvmParallelJobs
FROM alpine:3.14.2
LABEL maintainer "Symbolica <dev@symbolica.dev>"
ENV HOME /home/me

RUN llvmParallelJobs="${llvmParallelJobs:-$(nproc)}" && \
    apk add --no-cache binutils && \
    apk add --no-cache --virtual build-deps build-base cmake git ninja python3 && \
    cd /tmp && \
    git clone --depth 1 --branch llvmorg-12.0.1 https://github.com/llvm/llvm-project.git && \
    cmake -G Ninja \
        -Wno-dev \
        -S llvm-project/llvm \
        -B llvm-project/build \
        -DCLANG_DEFAULT_RTLIB=compiler-rt \
        -DCLANG_VENDOR=Alpine \
        -DCMAKE_BUILD_TYPE=MinSizeRel \
        -DCOMPILER_RT_BUILD_CRT=ON \
        -DCOMPILER_RT_BUILD_LIBFUZZER=OFF \
        -DCOMPILER_RT_BUILD_MEMPROF=OFF \
        -DCOMPILER_RT_BUILD_SANITIZERS=OFF \
        -DCOMPILER_RT_BUILD_XRAY=OFF \
        -DCOMPILER_RT_USE_BUILTINS_LIBRARY=OFF \
        -DCOMPILER_RT_USE_LIBCXX=OFF \
        -DLLVM_ENABLE_PROJECTS=clang \
        -DLLVM_ENABLE_RUNTIMES=compiler-rt \
        -DLLVM_HOST_TRIPLE=x86_64-alpine-linux-musl \
        -DLLVM_INCLUDE_EXAMPLES=OFF \
        -DLLVM_INCLUDE_TESTS=OFF \
        -DLLVM_PARALLEL_LINK_JOBS=${llvmParallelJobs} \
        -DLLVM_TARGETS_TO_BUILD=Native && \
    ninja -C llvm-project/build -j ${llvmParallelJobs} install-clang install-compiler-rt install-llvm-link && \
    mv /usr/local/bin/llvm-link /usr/local/bin/llvm-link-12 && \
    rm -rf /tmp/* && \
    apk --purge del build-deps

COPY . $HOME/.symbolica

# Install findutils because the default busybox xargs doesnt handle large inputs well
# and it ends up batching the calls to llvm-link which results in us only producing
# a libc.bc file with the contents of the final invocation in it.
RUN apk add --no-cache --virtual musl-build-deps findutils git make && \
    $HOME/.symbolica/install-musl.sh && \
    rm -rf /tmp/* $HOME/.symbolica/install-musl.sh && \
    apk --purge del musl-build-deps && \
    $HOME/.symbolica/test-cc.sh && \
    rm $HOME/.symbolica/test-cc.sh

WORKDIR /code
ENTRYPOINT [ "./symbolica.sh" ]
