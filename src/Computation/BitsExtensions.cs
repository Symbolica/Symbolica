using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
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

            return ConstantUnsigned.Create(self, nan).Constant;
        }

        public static IFloat ParseFloat(this Bits self, string value)
        {
            return (uint) self switch
            {
                32U => new ConstantSingle(float.Parse(value)),
                64U => new ConstantDouble(double.Parse(value)),
                _ => new SymbolicFloat(self, c => self.ParseNormalFloat(c, value))
            };
        }

        public static FPExpr ParseNormalFloat(this Bits self, Context context, string value)
        {
            var sort = self.GetSort(context);
            var (sign, significand, exponent) = ParseDecimal(value);

            return significand.IsZero
                ? context.MkFPZero(sort, sign)
                : CreateFloat(context, sort, sign, significand, exponent);
        }

        private static FPExpr CreateFloat(Context context, FPSort sort, bool sign, BigInteger significand, int exponent)
        {
            var (numerator, denominator) = exponent < 0
                ? (significand, BigInteger.Pow(10, -exponent))
                : (significand * BigInteger.Pow(10, exponent), BigInteger.One);

            var precision = (int) sort.SBits;
            var scale = precision - 1L;

            var lower = BigInteger.One << (precision - 1);
            while (numerator / denominator < lower)
            {
                numerator <<= 1;
                --scale;
            }

            var upper = BigInteger.One << precision;
            while (upper <= numerator / denominator)
            {
                denominator <<= 1;
                ++scale;
            }

            var quotient = numerator / denominator;
            var remainder = numerator % denominator;
            var midpoint = denominator >> 1;

            var (rounded, shift) = remainder > midpoint || remainder == midpoint && !quotient.IsEven
                ? quotient == upper - BigInteger.One
                    ? (lower, scale + 1L)
                    : (quotient + BigInteger.One, scale)
                : (quotient, scale);

            return context.MkFP(sign, shift, (ulong) (rounded % lower), sort);
        }

        private static (bool, BigInteger, int) ParseDecimal(string value)
        {
            var sign = value[0] == '-';

            var (significand, exponent) = sign
                ? ParseNonNegativeDecimal(value[1..])
                : ParseNonNegativeDecimal(value);

            return (sign, significand, exponent);
        }

        private static (BigInteger, int) ParseNonNegativeDecimal(string value)
        {
            var index = value.IndexOfAny(new[] {'e', 'E'});

            return index == -1
                ? ParseStandardNonNegativeDecimal(value, 0)
                : ParseStandardNonNegativeDecimal(value[..index], int.Parse(value[(index + 1)..]));
        }

        private static (BigInteger, int) ParseStandardNonNegativeDecimal(string value, int exponent)
        {
            var index = value.IndexOf('.');

            return index == -1
                ? (BigInteger.Parse(value), exponent)
                : (BigInteger.Parse(value[..index] + value[(index + 1)..]), exponent - (value.Length - 1 - index));
        }
    }
}
