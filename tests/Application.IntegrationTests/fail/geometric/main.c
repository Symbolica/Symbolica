#include "symbolica.h"
#include <assert.h>
#include <math.h>

int geometric_sum(int a, int r, int n)
{
    int s = 0;
    for (int i = 0; i <= n; ++i) {
        s += a;
        a *= r;
    }
    return s;
}

int closed_form_geometric_sum(int a, int r, int n)
{
    return a * (1 - (int) pow(r, n + 1)) / (1 - r);
}

int main()
{
    int a;
    SYMBOLIZE(a);
    int r;
    SYMBOLIZE(r);
    int n;
    SYMBOLIZE(n);

    assert(geometric_sum(a, r, n) == closed_form_geometric_sum(a, r, n));

    return 0;
}
