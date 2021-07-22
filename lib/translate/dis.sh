#!/bin/bash
set -e

cd dis
clang++-8 main.cpp -o dis `llvm-config-8 --cxxflags --libs irreader support`
cd ..
