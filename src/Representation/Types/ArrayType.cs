using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Types;

public sealed class ArrayType : IType
{
    private readonly uint _count;
    private readonly IType _elementType;

    public ArrayType(uint count, IType elementType)
    {
        _count = count;
        _elementType = elementType;
    }

    public Bytes Size => _elementType.Size * _count;

    public IType GetType(ISpace space, IExpression index)
    {
        return _elementType;
    }

    public IExpression GetOffsetBits(ISpace space, IExpression index)
    {
        return GetOffset(space, index, (uint) _elementType.Size.ToBits());
    }

    public IExpression GetOffsetBytes(ISpace space, IExpression index)
    {
        return GetOffset(space, index, (uint) _elementType.Size);
    }

    private static IExpression GetOffset(ISpace space, IExpression index, BigInteger elementSize)
    {
        return space.CreateConstant(space.PointerSize, elementSize).Multiply(index.SignExtend(space.PointerSize));
    }
}
