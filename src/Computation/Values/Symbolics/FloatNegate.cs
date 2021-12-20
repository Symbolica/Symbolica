using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics
{
    internal sealed class FloatNegate : Float
    {
        private readonly IValue _value;

        public FloatNegate(IValue value)
            : base(value.Size)
        {
            _value = value;
        }

        public override FPExpr AsFloat(Context context)
        {
            return context.MkFPNeg(_value.AsFloat(context));
        }
    }
}
