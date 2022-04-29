using System;
using System.Collections.Generic;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Types;

public sealed class PointerType : IPointerType
{
    public PointerType(IType elementType)
    {
        ElementType = elementType;
    }

    public Bytes Size => throw new NotImplementedException();
    public IEnumerable<IType> Types => throw new NotImplementedException();
    public IEnumerable<Bytes> Offsets => throw new NotImplementedException();

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

    private static IExpression GetOffset(ISpace space, IExpression index, BigInteger elementSize)
    {
        return space.CreateConstant(space.PointerSize, elementSize).Multiply(index.SignExtend(space.PointerSize));
    }

    public IType Deferefence(Bytes allocatedSize)
    {
        IType CreateArrayType()
        {
            // We've got an array here so need to replicate the element type n times
            if ((uint) allocatedSize % (uint) ElementType.Size != 0)
                throw new Exception("Expected the type to be an array element, but doesnt seem to fit the allocation.");

            var elements = (uint) allocatedSize / (uint) ElementType.Size;
            return new ArrayType(elements, ElementType);
        }

        return ElementType.Size == allocatedSize
            ? ElementType
            : CreateArrayType();
    }
}
