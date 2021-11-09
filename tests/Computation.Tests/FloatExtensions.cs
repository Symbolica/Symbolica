using System.Numerics;

namespace Symbolica.Computation
{
    internal static class FloatExtensions
    {
        public static BigInteger AsSingleNanNormalizedConstant(this IFloat self, IContextFactory contextFactory)
        {
            return self.NotEqual(self).AsConstant(contextFactory).IsZero
                ? self.AsConstant(contextFactory)
                : new ConstantSingle(float.NaN).AsConstant(contextFactory);
        }

        public static BigInteger AsDoubleNanNormalizedConstant(this IFloat self, IContextFactory contextFactory)
        {
            return self.NotEqual(self).AsConstant(contextFactory).IsZero
                ? self.AsConstant(contextFactory)
                : new ConstantDouble(double.NaN).AsConstant(contextFactory);
        }
    }
}
