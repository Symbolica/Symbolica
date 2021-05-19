using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    public sealed class SpaceFactory : ISpaceFactory
    {
        private readonly ICollectionFactory _collectionFactory;
        private readonly IModelFactory _modelFactory;
        private readonly ISymbolFactory _symbolFactory;

        public SpaceFactory(
            ISymbolFactory symbolFactory, IModelFactory modelFactory, ICollectionFactory collectionFactory)
        {
            _symbolFactory = symbolFactory;
            _modelFactory = modelFactory;
            _collectionFactory = collectionFactory;
        }

        public ISpace CreateInitial(Bits pointerSize, bool useSymbolicGarbage)
        {
            return PersistentSpace.Create(pointerSize, useSymbolicGarbage,
                _symbolFactory, _modelFactory, _collectionFactory);
        }
    }
}