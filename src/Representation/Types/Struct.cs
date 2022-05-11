using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Types;

internal sealed class Struct : IStruct
{
    private readonly IExpressionFactory _exprFactory;
    private readonly Bits[] _offsets;
    private readonly Bits[] _sizes;

    public Struct(IExpressionFactory exprFactory, Bits[] offsets, Bits[] sizes,
        IExpression expression)
    {
        _exprFactory = exprFactory;
        _offsets = offsets;
        _sizes = sizes;
        Expression = expression;
    }

    public IExpression Expression { get; }

    public IExpression Read(ISpace space, int index)
    {
        return Expression.Read(space, GetOffset(index), _sizes[index]);
    }

    public IStruct Write(ISpace space, int index, IExpression value)
    {
        return new Struct(_exprFactory, _offsets, _sizes,
            Expression.Write(space, GetOffset(index), value));
    }

    public IStruct Write(ISpace space, int index, BigInteger value)
    {
        return new Struct(_exprFactory, _offsets, _sizes,
            Expression.Write(space, GetOffset(index), _exprFactory.CreateConstant(_sizes[index], value)));
    }

    private IExpression GetOffset(int index)
    {
        return _exprFactory.CreateConstant(Expression.Size, (uint) _offsets[index]);
    }
}
