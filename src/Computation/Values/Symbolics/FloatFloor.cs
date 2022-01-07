using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics
{
    internal sealed class FloatFloor : Float
    {
        private readonly IValue _value;

        public FloatFloor(IValue value)
            : base(value.Size)
        {
            _value = value;
        }

        public override FPExpr AsFloat(Context context)
        {
            return context.MkFPRoundToIntegral(context.MkFPRTN(), _value.AsFloat(context));
        }
    }
}
