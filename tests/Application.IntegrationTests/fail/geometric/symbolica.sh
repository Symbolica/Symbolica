#!/bin/bash
set -e

~/.symbolica/build/cc *.c
~/.symbolica/build/ld *.bc -o symbolica.bc
