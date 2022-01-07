using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics
{
    internal sealed class FloatCeiling : Float
    {
        private readonly IValue _value;

        public FloatCeiling(IValue value)
            : base(value.Size)
        {
            _value = value;
        }

        public override FPExpr AsFloat(Context context)
        {
            return context.MkFPRoundToIntegral(context.MkFPRTP(), _value.AsFloat(context));
        }
    }
}
