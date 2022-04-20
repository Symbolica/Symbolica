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

    public Bits Size => _elementType.Size * _count;

    public IType GetType(ISpace space, IExpression index)
    {
        return _elementType;
    }

    public IExpression GetOffsetBits(ISpace space, IExpression index)
    {
        return GetOffset(space, index, (uint) _elementType.Size);
    }

    public IExpression GetOffsetBytes(ISpace space, IExpression index)
    {
        return GetOffset(space, index, (uint) _elementType.Size.ToBytes());
    }

    private static IExpression GetOffset(ISpace space, IExpression index, BigInteger elementSize)
    {
        return index.SignExtend(space.PointerSize).Multiply(space.CreateConstant(space.PointerSize, elementSize));
    }
}
