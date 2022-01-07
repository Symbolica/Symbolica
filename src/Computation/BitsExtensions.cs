using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal static class BitsExtensions
    {
        public static FPSort GetSort(this Bits self, Context context)
        {
            return (uint) self switch
            {
                16U => context.MkFPSort16(),
                32U => context.MkFPSort32(),
                64U => context.MkFPSort64(),
                80U => context.MkFPSort(15U, 65U),
                128U => context.MkFPSort128(),
                _ => throw new UnsupportedFloatingPointTypeException(self)
            };
        }

        public static BigInteger GetNan(this Bits self, Context context)
        {
            var sort = self.GetSort(context);
            var nan = ((BigInteger.One << ((int) sort.EBits + 2)) - BigInteger.One) << ((int) sort.SBits - 2);

            return ConstantUnsigned.Create(self, nan);
        }
    }
}
