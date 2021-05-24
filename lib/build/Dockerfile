FROM ubuntu:18.04
RUN apt-get update && \
    apt-get install -y apt-transport-https git gnupg software-properties-common wget && \
    wget -O - https://apt.llvm.org/llvm-snapshot.gpg.key | apt-key add - && \
    add-apt-repository "deb http://apt.llvm.org/bionic/ llvm-toolchain-bionic-8 main" && \
    apt-get install -y clang-8 make && \
    apt-get remove --purge -y software-properties-common wget && \
    apt-get autoremove --purge -y && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/* && \
    echo '#!/bin/bash \n$(which clang-8) "$@"' > /usr/bin/clang && \
    chmod a+x /usr/bin/clang && \
    echo '#!/bin/bash \n$(which llvm-link-8) "$@"' > /usr/bin/llvm-link && \
    chmod a+x /usr/bin/llvm-link

COPY cc musl-clang /usr/bin/
COPY atomic_arch.h /lib/symbolica/
RUN git clone --depth 1 --branch v1.2.2 git://git.musl-libc.org/musl
RUN cd musl && \
    touch crt/crtbegin.c crt/crtend.c crt/crtbeginS.c crt/crtendS.c crt/crtbeginT.c crt/crtendT.c && \
    cp /lib/symbolica/atomic_arch.h arch/x86_64 && \
    rm -r src/fenv/x86_64 src/ldso src/math/x86_64 src/process/x86_64 src/string/x86_64 && \
    CODEGEN=1 NO_LLVM=1 CC=cc CFLAGS="-O0 -Xclang -disable-O0-optnone -fPIC -U__GNUC__" ./configure --disable-shared --prefix=/install && \
    CODEGEN=1 make -j$(nproc) && \
    CODEGEN=1 NO_LLVM=1 make install && \
    find obj/src -name "*.lo.bc" ! -path "obj/src/thread/x86_64/*" ! -name setjmp.lo.bc ! -name longjmp.lo.bc ! -name sigsetjmp.lo.bc ! -name restore.lo.bc -print0 | xargs -0 llvm-link-8 -o /lib/symbolica/libc.bc && \
    cd ..

WORKDIR /code
ENTRYPOINT [ "./symbolica.sh" ]
