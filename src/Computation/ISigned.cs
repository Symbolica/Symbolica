using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal interface ISigned : IValue
    {
        IFunc<Context, BitVecExpr> Symbolic { get; }

        ISigned ArithmeticShiftRight(IUnsigned value);
        ISigned Divide(ISigned value);
        IBool Greater(ISigned value);
        IBool GreaterOrEqual(ISigned value);
        IBool Less(ISigned value);
        IBool LessOrEqual(ISigned value);
        ISigned Remainder(ISigned value);
        IFloat SignedToFloat(Bits size);
        ISigned SignExtend(Bits size);
    }
}
