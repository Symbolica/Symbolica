using System;
using Microsoft.Z3;

namespace Symbolica.Computation;

public sealed class DisposableContext : IContext
{
    private readonly Context _context;

    private DisposableContext(Context context)
    {
        _context = context;
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

    public static IContext Create()
    {
        return new DisposableContext(new Context());
    }
}
