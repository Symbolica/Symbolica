#include <assert.h>
#include <symbolica.h>

int sign(int x)
{
    return x > 0 ? 1 : x < 0 ? -1
                             : 0;
}

int sign_trick(int x)
{
    return (x > 0) - (x < 0);
}

int main()
{
    int x;
    SYMBOLIZE(x);

    int y0 = sign(x);
    int y1 = sign_trick(x);

    assert(y0 == y1);

    return 0;
}
