using Microsoft.Z3;
using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Computation;

internal sealed class AsBitVector : IExprMapper<BitVecExpr>
{
    private readonly ISolver _solver;

    public AsBitVector(ISolver solver)
    {
        _solver = solver;
    }

    public BoolExpr AsBool(IBitVector expression)
    {
        using var bitVector = expression.Map(this);
        using var zero = _solver.Context.MkBV(0U, (uint) expression.Size);
        using var isZero = _solver.Context.MkEq(bitVector, zero);
        return _solver.Context.MkNot(isZero);
    }

    public BitVecExpr Map(IConstantValue constant)
    {
        return constant.Map(new ConstantAsBitVector());
    }

    public BitVecExpr Map<U>(IUnaryExpr<U> unary) where U : IExpression
    {
        return unary.Map(new UnaryAsBitVector());
    }

    public BitVecExpr Map(IBinaryExpr binary)
    {
        return binary.Map(new BinaryAsBitVector());
    }

    public BitVecExpr Map(Address address)
    {
        return address.Aggregate().Map(this);
    }

    public BitVecExpr Map(Select select)
    {
        using var predicate = select.Predicate.Map(new AsBool(_solver));
        using var trueValue = select.TrueValue.Map(this);
        using var falseValue = select.FalseValue.Map(this);
        return (BitVecExpr) _solver.Context.MkITE(predicate, trueValue, falseValue);
    }

    public BitVecExpr Map(Expression.Symbol symbol)
    {
        _solver.Assert(symbol.Name, symbol.Assertions);

        return _solver.Context.MkBVConst(symbol.Name, (uint) symbol.Size);
    }

    private sealed class ConstantAsBitVector : IConstantMapper<BitVecExpr>
    {

    }

    private sealed class UnaryAsBitVector : IUnaryExprMapper<BitVecExpr>
    {

    }

    private sealed class BinaryAsBitVector : IBinaryExprMapper<BitVecExpr>
    {

    }
}
