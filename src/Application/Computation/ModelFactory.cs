using System.Collections.Generic;

namespace Symbolica.Computation
{
    internal sealed class ModelFactory : IModelFactory
    {
        public IModel Create(IContextFactory contextFactory, IEnumerable<IValue> assertions)
        {
            return Model.Create(contextFactory, assertions);
        }
    }
}
