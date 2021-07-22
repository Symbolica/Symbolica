FROM ubuntu:18.04
RUN apt-get update && \
    apt-get install -y apt-transport-https git gnupg software-properties-common wget && \
    wget -O - https://apt.llvm.org/llvm-snapshot.gpg.key | apt-key add - && \
    add-apt-repository "deb http://apt.llvm.org/bionic/ llvm-toolchain-bionic-8 main" && \
    apt-get update && \
    apt-get install -y clang-8 make && \
    apt-get remove --purge -y software-properties-common wget && \
    apt-get autoremove --purge -y && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

ENV HOME /home/me
WORKDIR $HOME/.symbolica/build
COPY . .
RUN ./musl.sh

WORKDIR /code
ENTRYPOINT [ "./symbolica.sh" ]
