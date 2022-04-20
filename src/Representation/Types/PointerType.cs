using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Types;

public sealed class PointerType : IType
{
    private readonly IType _elementType;

    public PointerType(Bits size, IType elementType)
    {
        Size = size;
        _elementType = elementType;
    }

    public Bits Size { get; }

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
