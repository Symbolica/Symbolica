using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics
{
    internal sealed class Truncate : BitVector
    {
        private readonly IValue _value;

        public Truncate(Bits size, IValue value)
            : base(size)
        {
            _value = value;
        }

        public override BitVecExpr AsBitVector(Context context)
        {
            return context.MkExtract((uint) (Size - Bits.One), 0U, _value.AsBitVector(context));
        }
    }
}
