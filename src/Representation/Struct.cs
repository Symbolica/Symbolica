using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation;

internal sealed class Struct : IStruct
{
    private readonly Expression.Offset[] _offsets;
    private readonly Bits[] _sizes;

    public Struct(Expression.Offset[] offsets, Bits[] sizes,
        IExpression<IType> expression)
    {
        _offsets = offsets;
        _sizes = sizes;
        Expression = expression;
    }

    public IExpression<IType> Expression { get; }

    public IExpression<IType> Read(ISpace space, int index)
    {
        return space.Read(Expression, GetOffset(space, index), _sizes[index]);
    }

    public IStruct Write(ISpace space, int index, IExpression<IType> value)
    {
        return new Struct(_offsets, _sizes,
            space.Write(Expression, GetOffset(space, index), value));
    }

    public IStruct Write(ISpace space, int index, BigInteger value)
    {
        return new Struct(_offsets, _sizes,
            space.Write(Expression, GetOffset(space, index), ConstantUnsigned.Create(_sizes[index], value)));
    }

    private Address GetOffset(ISpace space, int index)
    {
        return Address.CreateNull(space.PointerSize).AppendOffsets(_offsets[index]);
    }
}
