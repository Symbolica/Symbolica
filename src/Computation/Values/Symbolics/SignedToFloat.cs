using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics
{
    internal sealed class SignedToFloat : Float
    {
        private readonly IValue _value;

        public SignedToFloat(Bits size, IValue value)
            : base(size)
        {
            _value = value;
        }

        public override FPExpr AsFloat(Context context)
        {
            return context.MkFPToFP(context.MkFPRNE(), _value.AsBitVector(context), Size.GetSort(context), true);
        }
    }
}
