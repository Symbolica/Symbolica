#!/usr/bin/env sh
set -e

rm -rf ~/.symbolica
mkdir ~/.symbolica
cp -r lib/c/build/. lib/translate/. ~/.symbolica

cd ~/.symbolica
./install-musl.sh
