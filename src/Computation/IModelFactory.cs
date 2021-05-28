using System;
using System.Collections.Generic;
using Microsoft.Z3;

namespace Symbolica.Computation
{
    public interface IModelFactory
    {
        IModel Create(IEnumerable<Func<Context, BoolExpr>> assertions);
    }
}
