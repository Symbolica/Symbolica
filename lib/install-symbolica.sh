#!/bin/bash
set -e

rm -rf ~/.symbolica

cp -r lib/translate ~/.symbolica
cp -r lib/build ~/.symbolica/build

cd ~/.symbolica
./dis.sh

cd ~/.symbolica/build
./musl.sh