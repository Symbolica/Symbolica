using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Exceptions;

namespace Symbolica.Computation;

internal static class BitVecExprExtensions
{
    public static BigInteger AsConstant(this BitVecExpr self)
    {
        var expr = self.Simplify();

        return expr.IsNumeral
            ? ((BitVecNum) expr).BigInteger
            : throw new IrreducibleSymbolicExpressionException();
    }

    public static BoolExpr AsBool(this BitVecExpr self, IContext context)
    {
        return context.Execute(c => c.MkNot(c.MkEq(self, c.MkBV(0U, self.SortSize))));
    }
}
