FROM alpine:3.14.2

RUN apk add --no-cache build-base cmake git ninja python3 && \
    mkdir -p /tmp/clang/install && \
    cd /tmp/clang && \
    git clone --depth 1 --branch llvmorg-12.0.1 https://github.com/llvm/llvm-project.git && \
    mkdir build

WORKDIR /tmp/clang/build
RUN cmake -G Ninja \
        -Wno-dev \
        -DCMAKE_INSTALL_PREFIX=/tmp/clang/install \
        -DCLANG_VENDOR=Alpine \
        -DLLVM_ENABLE_PROJECTS=clang \
        -DCMAKE_BUILD_TYPE=MinSizeRel \
        -DLLVM_TARGETS_TO_BUILD=Native \
        -DLLVM_PARALLEL_LINK_JOBS=4 \
        -DLIBCLANG_BUILD_STATIC=ON \
        -DLLVM_DEFAULT_TARGET_TRIPLE=x86_64-alpine-linux-musl \
        -DLLVM_HOST_TRIPLE=x86_64-alpine-linux-musl \
        -DLLVM_ENABLE_CXX1Y=YES \
        -DLLVM_ENABLE_EH=ON \
        -DLLVM_ENABLE_FFI=NO \
        -DLLVM_ENABLE_LIBCXX=NO \
        -DLLVM_ENABLE_PIC=YES \
        -DLLVM_ENABLE_RTTI=YES \
        /tmp/clang/llvm-project/llvm
RUN ninja -j 4 install-llvm-extract
RUN ninja -j 4 install-llvm-link
RUN ninja -j 4 install-opt
RUN ninja -j 4 install-clang
