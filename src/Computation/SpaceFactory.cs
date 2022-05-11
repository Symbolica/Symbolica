using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation;

public sealed class SpaceFactory : ISpaceFactory
{
    private readonly ICollectionFactory _collectionFactory;

    public SpaceFactory(ICollectionFactory collectionFactory)
    {
        _collectionFactory = collectionFactory;
    }

    public ISpace CreateInitial()
    {
        return PersistentSpace.Create(_collectionFactory);
    }
}
