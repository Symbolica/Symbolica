using System.Numerics;
using Microsoft.Z3;

namespace Symbolica.Computation
{
    internal static class ValueExtensions
    {
        public static BigInteger GetSingleNanNormalizedInteger(this IValue self, Context context)
        {
            return self.FloatNotEqual(self).GetInteger(context).IsZero
                ? self.GetInteger(context)
                : new ConstantSingle(float.NaN).GetInteger(context);
        }

        public static BigInteger GetDoubleNanNormalizedInteger(this IValue self, Context context)
        {
            return self.FloatNotEqual(self).GetInteger(context).IsZero
                ? self.GetInteger(context)
                : new ConstantDouble(double.NaN).GetInteger(context);
        }
    }
}
