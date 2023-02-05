// An example highlighting how Symbolica can test the equivalence of two functions.
// Tests that two different implementations of a sign function are equivalent for all inputs.

#include <assert.h>
#include "symbolica.h"

int sign(int x)
{
    if (x < 0)
        return -1;
    else if (x > 1)
        return 1;
    else
        return 0;
}

int sign_trick(int x)
{
    return (x > 0) - (x < 0);
}

int main()
{
    int x;

    // Marks x as a symbol rather than a concrete value.
    SYMBOLIZE(x);

    int y0 = sign(x);
    int y1 = sign_trick(x);

    // Symbolica will check that this assertion holds for all possible values
    assert(y0 == y1);

    return 0;
}