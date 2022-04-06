using Microsoft.Z3;
using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Computation;

internal sealed class AsReal : ITypeMapper<RealExpr>
{
    private readonly ISolver _solver;

    public AsReal(ISolver solver)
    {
        _solver = solver;
    }

    public RealExpr Map(IIntegerExpression integer)
    {
        return MapNonReal(integer);
    }

    public RealExpr Map(IFloatExpression @float)
    {
        return MapNonReal(@float);
    }

    public RealExpr Map(IRealExpression real)
    {
        return real.Map(new RealToReal(_solver));
    }

    public RealExpr Map(Select select)
    {
        return MapNonReal(select);
    }

    private RealExpr MapNonReal(IExpression<IType> expression)
    {
        using var flt = expression.Map(new AsFloat(_solver));
        return _solver.Context.MkFPToReal(flt);
    }

    private sealed class RealToReal : IRealMapper<RealExpr>
    {
        private readonly ISolver _solver;

        public RealToReal(ISolver solver)
        {
            _solver = solver;
        }

        public RealExpr Map(FloatPower floatPower)
        {
            using var left = floatPower.Left.Map(new AsReal(_solver));
            using var right = floatPower.Right.Map(new AsReal(_solver));

            return (RealExpr) _solver.Context.MkPower(left, right);
        }

        public RealExpr Map(RealConvert realConvert)
        {
            return realConvert.Value.Map(new AsReal(_solver));
        }
    }
}
