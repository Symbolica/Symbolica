using System;
using Microsoft.Z3;

namespace Symbolica.Computation;

public interface IContext : IDisposable
{
    TResult Execute<TResult>(Func<Context, TResult> func)
        where TResult : Z3Object;
}
