#!/usr/bin/env sh
set -e

~/.symbolica/bin/cc sign_trick.c
~/.symbolica/bin/libc-link *.bc -o symbolica.bc