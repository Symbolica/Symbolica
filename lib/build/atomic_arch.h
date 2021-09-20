// TODO: Is this only needed at install time?

#define a_cas a_cas
static inline int a_cas(volatile int *p, int t, int s)
{
    int old = *p;
    if (old == t)
        *p = s;
    return old;
}

#define a_cas_p a_cas_p
static inline void *a_cas_p(volatile void *p, void *t, void *s)
{
    void *old = *(void *volatile *)p;
    if (old == t)
        *(void *volatile *)p = s;
    return old;
}
