using System.Collections.Generic;

namespace Symbolica.Computation;

public interface IModelFactory
{
    IModel Create(IContextFactory contextFactory, IEnumerable<IValue> assertions);
}
