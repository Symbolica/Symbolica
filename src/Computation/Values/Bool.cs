using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values
{
    internal abstract class Bool : Integer
    {
        protected Bool()
            : base(Bits.One)
        {
        }

        public sealed override BitVecExpr AsBitVector(Context context)
        {
            return (BitVecExpr) context.MkITE(AsBool(context), context.MkBV(new[] {true}), context.MkBV(new[] {false}));
        }
    }
}
