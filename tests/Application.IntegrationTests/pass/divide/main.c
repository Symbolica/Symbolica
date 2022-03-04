#include <assert.h>
#include <stdint.h>
#include <symbolica.h>

int main()
{
    uint8_t x;
    SYMBOLIZE(x);
    uint8_t y;
    SYMBOLIZE(y);

    if (y == 0)
        return 0;

    uint8_t q = x / y;
    uint8_t r = x % y;

    assert(x == q * y + r);

    return 0;
}
