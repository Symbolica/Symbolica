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

ENV HOME /home/me
WORKDIR $HOME/.symbolica
COPY . .
RUN ./dis.sh

FROM llvm-base as runtime
RUN apt-get update && \
    apt-get install -y llvm-8 && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

ENV HOME /home/me
WORKDIR $HOME/.symbolica
COPY --from=runtime-builder $HOME/.symbolica/dis/dis dis/dis
COPY translate /.symbolica/

WORKDIR /code
ENTRYPOINT [ "/.symbolica/translate" ]
