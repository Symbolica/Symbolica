using System.Collections.Generic;
using Symbolica.Computation;

namespace Symbolica.Application.Computation
{
    internal sealed class ModelFactory : IModelFactory
    {
        public IModel Create(IContextFactory contextFactory, IEnumerable<IValue> assertions)
        {
            return Model.Create(contextFactory, assertions);
        }
    }
}
