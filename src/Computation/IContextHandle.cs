using System;
using Microsoft.Z3;

namespace Symbolica.Computation
{
    public interface IContextHandle : IDisposable
    {
        Context Context { get; }
    }
}
