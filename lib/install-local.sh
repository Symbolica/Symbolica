#!/bin/bash
set -e

rm -rf ~/.symbolica
mkdir ~/.symbolica

cp -r lib/build ~/.symbolica/build
cd ~/.symbolica/build
./musl.sh

cp -r lib/translate ~/.symbolica
cd ~/.symbolica
./dis.sh
