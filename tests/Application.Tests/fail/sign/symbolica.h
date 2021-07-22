#include <stddef.h>

#define SYMBOLIZE(x) symbolize(&x, sizeof x, #x)

void symbolize(void *address, size_t size, const char *name);
