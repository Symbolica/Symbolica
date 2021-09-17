FROM alpine:3.14.2 as builder

RUN apk add --no-cache build-base cmake git ninja python3

RUN mkdir -p /tmp/clang/install && \
    cd /tmp/clang && \
    git clone --depth 1 --branch llvmorg-12.0.1 https://github.com/llvm/llvm-project.git && \
    mkdir build && \
    cd build

WORKDIR /tmp/clang/build
# RUN apk add --no-cache lld
RUN cmake -G Ninja \
        -Wno-dev \
        -DCMAKE_INSTALL_PREFIX=/tmp/clang/install \
        -DCLANG_VENDOR=Alpine \
        -DLLVM_ENABLE_PROJECTS=clang \
        -DCMAKE_BUILD_TYPE=MinSizeRel \
        -DLLVM_TARGETS_TO_BUILD=Native \
        # -DLLVM_USE_LINKER=lld \
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
        # -DCLANG_ENABLE_BOOTSTRAP=ON \
        # -DBOOTSTRAP_CMAKE_BUILD_TYPE=MinSizeRel \
        # -DCLANG_BOOTSTRAP_TARGETS="install-clang;install-clang-resource-headers" \
        /tmp/clang/llvm-project/llvm
# RUN apk add --no-cache libstdc++
RUN ninja -j 4 clang
RUN ninja -j 4 install-clang
RUN ninja -j 4 install-llvm-link

FROM alpine:3.14.2
LABEL maintainer "Symbolica <dev@symbolica.dev>"
# # FIXME: Install all packages you want to have in your release container.
# # A minimal useful installation should include at least libstdc++ and binutils.
RUN apk add --no-cache binutils

COPY --from=builder /tmp/clang/install/ /usr/local/
