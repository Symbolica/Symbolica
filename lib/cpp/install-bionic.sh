#!/usr/bin/env sh
set -e

wget -O - https://apt.kitware.com/keys/kitware-archive-latest.asc | apt-key add -
add-apt-repository "deb https://apt.kitware.com/ubuntu/ bionic main"

apt-get update
apt-get install -y cmake ninja-build
