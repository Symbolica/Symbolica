using System.Numerics;
using Microsoft.Z3;

namespace Symbolica.Computation
{
    internal static class FloatExtensions
    {
        public static BigInteger AsSingleNanNormalizedConstant(this IFloat self, Context context)
        {
            return self.NotEqual(self).AsConstant(context).IsZero
                ? self.AsConstant(context)
                : new ConstantSingle(float.NaN).AsConstant(context);
        }

        public static BigInteger AsDoubleNanNormalizedConstant(this IFloat self, Context context)
        {
            return self.NotEqual(self).AsConstant(context).IsZero
                ? self.AsConstant(context)
                : new ConstantDouble(double.NaN).AsConstant(context);
        }
    }
}
