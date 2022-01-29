using System.Numerics;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal static class ExpressionExtensions
{
    public static BigInteger GetNanNormalizedConstant(this IExpression self, IContextFactory contextFactory)
    {
        using var handle = contextFactory.Create();

        return self.FloatNotEqual(self).Constant.IsZero
            ? self.Constant
            : self.Size.GetNan(handle.Context);
    }
}
