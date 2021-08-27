#!/bin/bash
set -e

wget -O - https://apt.llvm.org/llvm-snapshot.gpg.key | apt-key add -
add-apt-repository "deb http://apt.llvm.org/focal/ llvm-toolchain-focal-12 main"

apt-get update
apt-get install -y clang-12 make
