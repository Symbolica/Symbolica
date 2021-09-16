FROM alpine:3.14.2 as builder

RUN apk add --no-cache build-base cmake git ninja python3

RUN mkdir -p /tmp/clang/install && \
    cd /tmp/clang && \
    git clone --depth 1 --branch llvmorg-12.0.1 https://github.com/llvm/llvm-project.git && \
    mkdir build && \
    cd build

WORKDIR /tmp/clang/build
RUN apk add --no-cache binutils binutils-gold
RUN cmake -GNinja \
        -DCMAKE_INSTALL_PREFIX=/tmp/clang/install \
        -DLLVM_ENABLE_PROJECTS=clang \
        -DCMAKE_BUILD_TYPE=MinSizeRel \
        -DLLVM_TARGETS_TO_BUILD=Native \
        -DBOOTSTRAP_CMAKE_BUILD_TYPE=MinSizeRel \
        -DLLVM_USE_LINKER=gold \
        -DLLVM_PARALLEL_LINK_JOBS=4 \
        -DCLANG_ENABLE_BOOTSTRAP=ON \
        -DCLANG_BOOTSTRAP_TARGETS="install-clang;install-clang-resource-headers" \
        /tmp/clang/llvm-project/llvm
RUN ninja -j 4 stage2-install-clang stage2-install-clang-resource-headers

# FROM alpine:3.14.2
# LABEL maintainer "Symbolica <dev@symbolica.dev>"
# # FIXME: Install all packages you want to have in your release container.
# # A minimal useful installation should include at least libstdc++ and binutils.
# RUN apk add --no-cache binutils

# COPY --from=builder /tmp/clang/install/ /usr/local/
