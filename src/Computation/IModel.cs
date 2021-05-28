using System;
using System.Collections.Generic;
using Microsoft.Z3;

namespace Symbolica.Computation
{
    public interface IModel : IDisposable
    {
        bool IsSatisfiable(Func<Context, BoolExpr> assertion);
        Expr Evaluate(Func<Context, Expr> func);
        IEnumerable<KeyValuePair<string, string>> Evaluate();
    }
}
