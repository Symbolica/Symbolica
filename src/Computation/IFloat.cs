using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface IFloat : IValue
    {
        Func<Context, FPExpr> Symbolic { get; }

        IFloat Add(IFloat value);
        IFloat Ceiling();
        IFloat Convert(Bits size);
        IFloat Divide(IFloat value);
        IBool Equal(IFloat value);
        IFloat Floor();
        IBool Greater(IFloat value);
        IBool GreaterOrEqual(IFloat value);
        IBool Less(IFloat value);
        IBool LessOrEqual(IFloat value);
        IFloat Multiply(IFloat value);
        IFloat Negate();
        IBool NotEqual(IFloat value);
        IBool Ordered(IFloat value);
        IFloat Power(IFloat value);
        IFloat Remainder(IFloat value);
        IFloat Subtract(IFloat value);
        ISigned ToSigned(Bits size);
        IUnsigned ToUnsigned(Bits size);
        IBool Unordered(IFloat value);
    }
}
