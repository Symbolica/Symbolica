using System;
using Microsoft.Z3;

namespace Symbolica.Computation;

internal sealed class DisposableContext<TContextHandle> : IContext
    where TContextHandle : IContextHandle, new()
{
    private readonly TContextHandle _contextHandle;

    public DisposableContext()
    {
        _contextHandle = new TContextHandle();
    }

    public void Dispose()
    {
        _contextHandle.Dispose();
    }

    public Solver CreateSolver(Func<Context, Solver> func)
    {
        return func(_contextHandle.Context);
    }

    public TSort CreateSort<TSort>(Func<Context, TSort> func)
        where TSort : Sort
    {
        return func(_contextHandle.Context);
    }

    public TExpr CreateExpr<TExpr>(Func<Context, TExpr> func)
        where TExpr : Expr
    {
        return func(_contextHandle.Context);
    }
}
