using Microsoft.Z3;

namespace Symbolica.Computation
{
    internal static class ContextExtensions
    {
        public static FPExpr MakeFloat(this Context self, string value, FPSort sort)
        {
            var (sign, exponent, significand) = Parse(value);

            return self.MkFP(sign, exponent, significand, sort);
        }

        private static (bool, long, ulong) Parse(string value)
        {
            return value[0] == '-'
                ? Parse(true, value[1..])
                : Parse(false, value);
        }

        private static (bool, long, ulong) Parse(bool sign, string value)
        {
            var index = value.IndexOfAny(new[] {'e', 'E'});

            return index == -1
                ? Parse(sign, 0L, value)
                : Parse(sign, long.Parse(value[(index + 1)..]), value[..index]);
        }

        private static (bool, long, ulong) Parse(bool sign, long exponent, string value)
        {
            var index = value.IndexOf('.');

            return index == -1
                ? (sign, exponent, ulong.Parse(value))
                : (sign, exponent - (value.Length - 1L - index), ulong.Parse(value[..index] + value[(index + 1)..]));
        }
    }
}
