using System;
using Microsoft.Z3;

namespace Symbolica.Computation;

public sealed class DisposableContext : IContext
{
    private readonly Context _context;

    public DisposableContext()
    {
        _context = new Context();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public TResult Execute<TResult>(Func<Context, TResult> func)
        where TResult : Z3Object
    {
        return func(_context);
    }
}
