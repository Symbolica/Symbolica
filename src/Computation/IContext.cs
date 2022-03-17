using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Z3;

namespace Symbolica.Computation;

internal interface IContext : IDisposable
{
    void Assert(IEnumerable<BoolExpr> assertions);
    void Assert(string name, IEnumerable<BoolExpr> assertions);
    bool IsSatisfiable(BoolExpr assertion);
    BigInteger GetSingleValue(BitVecExpr variable);
    BigInteger GetExampleValue(BitVecExpr variable);
    IEnumerable<KeyValuePair<string, string>> GetExampleValues();

    TSort CreateSort<TSort>(Func<Context, TSort> func)
        where TSort : Sort;

    TExpr CreateExpr<TExpr>(Func<Context, TExpr> func)
        where TExpr : Expr;
}
