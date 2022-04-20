using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Types;

internal sealed class Struct : IStruct
{
    private readonly Bits[] _offsets;
    private readonly Bits[] _sizes;

    public Struct(Bits[] offsets, Bits[] sizes,
        IExpression expression)
    {
        _offsets = offsets;
        _sizes = sizes;
        Expression = expression;
    }

    public IExpression Expression { get; }

    public IExpression Read(ISpace space, int index)
    {
        return Expression.Read(GetOffset(space, index), _sizes[index]);
    }

    public IStruct Write(ISpace space, int index, IExpression value)
    {
        return new Struct(_offsets, _sizes,
            Expression.Write(GetOffset(space, index), value));
    }

    public IStruct Write(ISpace space, int index, BigInteger value)
    {
        return new Struct(_offsets, _sizes,
            Expression.Write(GetOffset(space, index), space.CreateConstant(_sizes[index], value)));
    }

    private IExpression GetOffset(ISpace space, int index)
    {
        return space.CreateConstant(Expression.Size, (uint) _offsets[index]);
    }
}
