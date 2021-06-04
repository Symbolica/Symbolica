FROM ubuntu:18.04 as llvm-base
RUN apt-get update && \
    apt-get install -y software-properties-common wget && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/* && \
    wget -O - https://apt.llvm.org/llvm-snapshot.gpg.key | apt-key add - && \
    add-apt-repository "deb http://apt.llvm.org/bionic/ llvm-toolchain-bionic-8 main" && \
    apt-get remove --purge -y software-properties-common wget && \
    apt-get autoremove --purge -y

FROM llvm-base as runtime-builder

RUN apt-get update && \
    apt-get install -y clang-8 && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

COPY dis /lib/symbolica/dis
RUN clang++-8 /lib/symbolica/dis/main.cpp -o /usr/bin/dis `llvm-config-8 --cxxflags --libs irreader support`

FROM llvm-base as runtime

RUN apt-get update && \
    apt-get install -y llvm-8 && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/* && \
    echo '#!/bin/bash \n$(which llvm-extract-8) "$@"' > /usr/bin/llvm-extract && \
    chmod a+x /usr/bin/llvm-extract && \
    echo '#!/bin/bash \n$(which opt-8) "$@"' > /usr/bin/opt && \
    chmod a+x /usr/bin/opt

COPY --from=runtime-builder /usr/bin/dis /usr/bin/dis
COPY translate.sh /lib/symbolica/

WORKDIR /code
ENTRYPOINT [ "/lib/symbolica/translate.sh" ]
