using System;
using Microsoft.Z3;

namespace Symbolica.Computation;

internal interface IContext : IDisposable
{
    Solver CreateSolver(Func<Context, Solver> func);

    TSort CreateSort<TSort>(Func<Context, TSort> func)
        where TSort : Sort;

    TExpr CreateExpr<TExpr>(Func<Context, TExpr> func)
        where TExpr : Expr;
}
