using Microsoft.Z3;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal static class SizeExtensions
{
    public static FPSort GetSort(this Size self, ISolver solver)
    {
        return self.Bits switch
        {
            16U => solver.Context.MkFPSort16(),
            32U => solver.Context.MkFPSort32(),
            64U => solver.Context.MkFPSort64(),
            80U => solver.Context.MkFPSort(15U, 65U),
            128U => solver.Context.MkFPSort128(),
            _ => throw new UnsupportedFloatingPointTypeException(self)
        };
    }
}
