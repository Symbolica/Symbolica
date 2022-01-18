#!/usr/bin/env sh
set -e

~/.symbolica/bin/cc *.c
~/.symbolica/bin/cc-link *.bc -o symbolica.bc
