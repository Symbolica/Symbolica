#!/usr/bin/env sh
set -e

git clone --depth 1 --branch v3.8.7 https://github.com/python/cpython.git
git clone --depth 1 --branch 0.29.21 https://github.com/cython/cython.git

cd cpython
sed -i '1i *static*' Modules/Setup
sed -i -r "/^#(\s|\*shared\*|readline|_testcapi|_testinternalcapi|_ssl|nis|_curses|_dbm|_gdbm|zlib|\*disabled\*|_sqlite3)/! s/^#//" Modules/Setup
CODEGEN=1 NO_LLVM=1 CC=~/.symbolica/bin/cc CFLAGS="-O0 -Xclang -disable-O0-optnone -fPIC" ./configure --disable-shared --without-pymalloc --without-ensurepip --prefix=$(eval echo ~/.symbolica)
CODEGEN=1 LINKFORSHARED=" " make
CODEGEN=1 NO_LLVM=1 make install
find -name "*.o.bc" ! -name _testembed.o.bc ! -name sysv.o.bc ! -name unix64.o.bc -print0 | xargs -0 ~/.symbolica/bin/llvm-link -o ~/.symbolica/lib/libpython.bc
cd ..

cd cython
CODEGEN=1 NO_LLVM=1 ~/.symbolica/bin/python3 setup.py install
cd ..
