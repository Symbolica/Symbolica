using System.Collections.Generic;

namespace Symbolica.Computation;

internal sealed class ModelFactory : IModelFactory
{
    public IModel Create<TContext>(IEnumerable<IValue> assertions)
        where TContext : IContext, new()
    {
        return Model<TContext>.Create(assertions);
    }
}
