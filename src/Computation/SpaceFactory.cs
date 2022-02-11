using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation;

public sealed class SpaceFactory<TContextHandle> : ISpaceFactory
    where TContextHandle : IContextHandle, new()
{
    private readonly ICollectionFactory _collectionFactory;

    public SpaceFactory(ICollectionFactory collectionFactory)
    {
        _collectionFactory = collectionFactory;
    }

    public ISpace CreateInitial(Bits pointerSize, bool useSymbolicGarbage)
    {
        return PersistentSpace<DisposableContext<TContextHandle>>.Create(
            pointerSize, useSymbolicGarbage, _collectionFactory);
    }
}
