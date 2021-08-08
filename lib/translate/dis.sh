#!/bin/bash
set -e

cd dis
clang++-12 main.cpp -o dis `llvm-config-12 --cxxflags --libs irreader support`
cd ..
