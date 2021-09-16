FROM ubuntu:18.04 as base
RUN apt-get update && \
    apt-get install -y software-properties-common wget && \
    wget -O - https://apt.llvm.org/llvm-snapshot.gpg.key | apt-key add - && \
    add-apt-repository "deb http://apt.llvm.org/bionic/ llvm-toolchain-bionic-12 main" && \
    apt-get update && \
    apt-get install -y clang-12 make && \
    apt-get remove --purge -y software-properties-common wget && \
    apt-get autoremove --purge -y && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

FROM base as builder
RUN apt-get update && \
    apt-get install -y apt-transport-https git gnupg && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

ENV HOME /home/me
WORKDIR $HOME/.symbolica/build
COPY . .
RUN ./musl.sh

FROM base
ENV HOME /home/me
WORKDIR $HOME/.symbolica/build
COPY --from=builder /home/me/.symbolica/build .

WORKDIR /code
ENTRYPOINT [ "./symbolica.sh" ]
