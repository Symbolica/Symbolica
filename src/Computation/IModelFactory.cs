using System.Collections.Generic;

namespace Symbolica.Computation;

public interface IModelFactory
{
    IModel Create<TContext>(IEnumerable<IValue> assertions)
        where TContext : IContext, new();
}
