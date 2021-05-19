using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation;

namespace Symbolica.Application.Computation
{
    internal sealed class ModelFactory : IModelFactory
    {
        public IModel Create(IEnumerable<Func<Context, BoolExpr>> assertions)
        {
            return Model.Create(assertions);
        }
    }
}