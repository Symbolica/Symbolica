using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation;

public sealed class SpaceFactory<TContext> : ISpaceFactory
    where TContext : IContext, new()
{
    private readonly ICollectionFactory _collectionFactory;

    public SpaceFactory(ICollectionFactory collectionFactory)
    {
        _collectionFactory = collectionFactory;
    }

    public ISpace CreateInitial(Bits pointerSize, bool useSymbolicGarbage)
    {
        return PersistentSpace<TContext>.Create(pointerSize, useSymbolicGarbage, _collectionFactory);
    }
}
