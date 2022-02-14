using System;
using System.Collections.Generic;
using Microsoft.Z3;

namespace Symbolica.Computation;

internal interface IContext : IDisposable
{
    Solver Solver { get; }

    void Assert(IEnumerable<BoolExpr> assertions);
    void Assert(string name, IEnumerable<BoolExpr> assertions);

    TSort CreateSort<TSort>(Func<Context, TSort> func)
        where TSort : Sort;

    TExpr CreateExpr<TExpr>(Func<Context, TExpr> func)
        where TExpr : Expr;
}
