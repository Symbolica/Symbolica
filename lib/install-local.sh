#!/bin/bash
set -e

rm -rf ~/.symbolica
mkdir ~/.symbolica

cp -r lib/build ~/.symbolica/build
cp -r lib/translate ~/.symbolica

cd ~/.symbolica/build
./musl.sh

cd ~/.symbolica
./dis.sh
