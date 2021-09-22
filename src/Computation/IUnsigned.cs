using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface IUnsigned : IBitwise
    {
        Func<Context, BitVecExpr> Symbolic { get; }

        IUnsigned Add(IUnsigned value);
        IUnsigned Divide(IUnsigned value);
        IBool Greater(IUnsigned value);
        IBool GreaterOrEqual(IUnsigned value);
        IBool Less(IUnsigned value);
        IBool LessOrEqual(IUnsigned value);
        IUnsigned LogicalShiftRight(IUnsigned value);
        IUnsigned Multiply(IUnsigned value);
        IUnsigned Not();
        IUnsigned Remainder(IUnsigned value);
        IUnsigned ShiftLeft(IUnsigned value);
        IUnsigned Subtract(IUnsigned value);
        IUnsigned Truncate(Bits size);
        IFloat UnsignedToFloat(Bits size);
        IUnsigned ZeroExtend(Bits size);
    }
}
