using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics
{
    internal sealed class RealConvert : Float, IRealValue
    {
        private readonly IRealValue _value;

        public RealConvert(Bits size, IRealValue value)
            : base(size)
        {
            _value = value;
        }

        public override FPExpr AsFloat(Context context)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public RealExpr AsReal(Context context)
        {
            return _value.AsReal(context);
        }
    }
}
