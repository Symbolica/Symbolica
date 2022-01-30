using Microsoft.Z3;

namespace Symbolica.Computation;

internal sealed class SharedContextHandle : IContextHandle
{
    public SharedContextHandle(Context context)
    {
        Context = context;
    }

    public Context Context { get; }

    public void Dispose()
    {
    }
}
