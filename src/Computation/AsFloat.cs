using System.Numerics;
using Microsoft.Z3;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Computation;

internal sealed class AsFloat : IExprMapper<FPExpr>
{
    private readonly ISolver _solver;

    public AsFloat(ISolver solver)
    {
        _solver = solver;
    }

    public BoolExpr AsBool(IFloat expression)
    {
        using var flt = expression.Map(this);
        using var isZero = _solver.Context.MkFPIsZero(flt);
        return _solver.Context.MkNot(isZero);
    }

    public BitVecExpr AsBitVector(IFloat expression)
    {
        using var flt = expression.Map(this);
        using var simplified = flt.Simplify();

        return simplified.IsFPNaN
            ? CreateNan(expression.Size)
            : _solver.Context.MkFPToIEEEBV(flt);
    }

    private BitVecExpr CreateNan(Bits size)
    {
        using var sort = size.GetSort(_solver);
        var nan = ((BigInteger.One << ((int) sort.EBits + 2)) - BigInteger.One) << ((int) sort.SBits - 2);

        return _solver.Context.MkBV(nan.ToString(), (uint) size);
    }
}
