using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal static class BitsExtensions
{
    public static FPSort GetSort(this Bits self, ISolver solver)
    {
        return (uint) self switch
        {
            16U => solver.Context.MkFPSort16(),
            32U => solver.Context.MkFPSort32(),
            64U => solver.Context.MkFPSort64(),
            80U => solver.Context.MkFPSort(15U, 65U),
            128U => solver.Context.MkFPSort128(),
            _ => throw new UnsupportedFloatingPointTypeException(self)
        };
    }

    public static BigInteger GetNan(this Bits self, ISolver solver)
    {
        using var sort = self.GetSort(solver);
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
