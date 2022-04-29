using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Types;

public sealed class ArrayType : IArrayType
{
    private readonly uint _count;

    public ArrayType(uint count, IType elementType)
    {
        _count = count;
        ElementType = elementType;
    }

    public Bytes Size => ElementType.Size * _count;
    public IEnumerable<IType> Types => Enumerable.Repeat(ElementType, (int) _count);
    public IEnumerable<Bytes> Offsets => Enumerable.Range(0, (int) _count).Select(i => ElementType.Size * (uint) i);

    public IType ElementType { get; }

    public IType GetType(ISpace space, IExpression index)
    {
        return ElementType;
    }

    public IExpression GetOffsetBits(ISpace space, IExpression index)
    {
        return GetOffset(space, index, (uint) ElementType.Size.ToBits());
    }

    public IExpression GetOffsetBytes(ISpace space, IExpression index)
    {
        return GetOffset(space, index, (uint) ElementType.Size);
    }

    public IType Resize(Bytes allocatedSize)
    {
        // We've got an array here so need to replicate the element type n times
        if ((uint) allocatedSize % (uint) ElementType.Size != 0)
            throw new Exception("Expected the type to be an array element, but doesnt seem to fit the allocation.");

        var elements = (uint) allocatedSize / (uint) ElementType.Size;
        return new ArrayType(elements, ElementType);
    }

    private static IExpression GetOffset(ISpace space, IExpression index, BigInteger elementSize)
    {
        return space.CreateConstant(space.PointerSize, elementSize).Multiply(index.SignExtend(space.PointerSize));
    }
}
