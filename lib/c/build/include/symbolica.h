#include <stddef.h>
#include <stdlib.h>

#define SYMBOLIZE(x) symbolize(&x, sizeof x, #x)

#ifdef __cplusplus
extern "C"
{
#endif

    void symbolize(void *address, size_t size, const char *name)
    {
        abort();
    }

#ifdef __cplusplus
}
#endif
