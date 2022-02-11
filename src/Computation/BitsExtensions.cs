using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal static class BitsExtensions
{
    public static FPSort GetSort(this Bits self, IContext context)
    {
        return context.CreateSort(c => (uint) self switch
        {
            16U => c.MkFPSort16(),
            32U => c.MkFPSort32(),
            64U => c.MkFPSort64(),
            80U => c.MkFPSort(15U, 65U),
            128U => c.MkFPSort128(),
            _ => throw new UnsupportedFloatingPointTypeException(self)
        });
    }

    public static BigInteger GetNan(this Bits self, IContext context)
    {
        var sort = self.GetSort(context);
        var nan = ((BigInteger.One << ((int) sort.EBits + 2)) - BigInteger.One) << ((int) sort.SBits - 2);

        return ConstantUnsigned.Create(self, nan);
    }

    public static IValue ParseFloat(this Bits self, string value)
    {
        return (uint) self switch
        {
            32U => new ConstantSingle(float.Parse(value)),
            64U => new ConstantDouble(double.Parse(value)),
            _ => new NormalFloat(self, value)
        };
    }
}
