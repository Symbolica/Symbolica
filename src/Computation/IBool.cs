using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface IBool : IBitwise
    {
        IFunc<Context, BoolExpr> Symbolic { get; }

        IProposition GetProposition(IPersistentSpace space, IBool[] constraints);
        IBool Not();
        IValue Select(IValue trueValue, IValue falseValue);
    }
}
