using Microsoft.Z3;

namespace Symbolica.Computation;

internal sealed class SharedContextFactory : IContextFactory
{
    private readonly Context _context;

    public SharedContextFactory()
    {
        _context = new Context();
    }

    public IContextHandle Create()
    {
        return new SharedContextHandle(_context);
    }
}
