using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics
{
    internal sealed class UnsignedLessOrEqual : Bool
    {
        private readonly IValue _left;
        private readonly IValue _right;

        public UnsignedLessOrEqual(IValue left, IValue right)
        {
            _left = left;
            _right = right;
        }

        public override BoolExpr AsBool(Context context)
        {
            return context.MkBVULE(_left.AsBitVector(context), _right.AsBitVector(context));
        }
    }
}
