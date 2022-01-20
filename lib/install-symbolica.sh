#!/usr/bin/env sh
set -e

rm -rf ~/.symbolica
mkdir ~/.symbolica
cp -r lib/c/build/. lib/cpp/build/. lib/translate/. ~/.symbolica

cd ~/.symbolica
./install-musl.sh
./install-libcxx.sh
