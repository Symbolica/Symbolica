#!/usr/bin/env sh
set -e

git clone --depth 1 --branch 0.29.21 https://github.com/cython/cython.git

cd cython
CODEGEN=1 NO_LLVM=1 ~/.symbolica/bin/python3 setup.py install
cd ..
