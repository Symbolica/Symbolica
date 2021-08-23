FROM ubuntu:18.04
RUN apt-get update && \
    apt-get install -y software-properties-common wget && \
    wget -O - https://apt.llvm.org/llvm-snapshot.gpg.key | apt-key add - && \
    add-apt-repository "deb http://apt.llvm.org/bionic/ llvm-toolchain-bionic-12 main" && \
    apt-get update && \
    apt-get install -y llvm-12 && \
    apt-get remove --purge -y software-properties-common wget && \
    apt-get autoremove --purge -y && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

COPY . /.symbolica/

WORKDIR /code
ENTRYPOINT [ "/.symbolica/translate" ]
