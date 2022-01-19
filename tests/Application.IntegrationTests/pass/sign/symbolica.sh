#!/usr/bin/env sh
set -e

~/.symbolica/bin/cc *.c
~/.symbolica/bin/libc-link *.bc -o symbolica.bc
