using System;
using Microsoft.Z3;

namespace Symbolica.Computation;

public interface IContextHandle : IDisposable
{
    public long RefCount { get; }
    Context Context { get; }
}
