using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Z3;

namespace Symbolica.Computation;

internal interface IContext : IDisposable
{
    void Assert(IEnumerable<IValue> assertions);
    void Assert(string name, IEnumerable<IValue> assertions);
    bool IsSatisfiable(IValue assertion);
    BigInteger GetSingleValue(IValue variable);
    BigInteger GetExampleValue(IValue variable);
    IEnumerable<KeyValuePair<string, string>> GetExampleValues();

    TSort CreateSort<TSort>(Func<Context, TSort> func)
        where TSort : Sort;

    TExpr CreateExpr<TExpr>(Func<Context, TExpr> func)
        where TExpr : Expr;
}
