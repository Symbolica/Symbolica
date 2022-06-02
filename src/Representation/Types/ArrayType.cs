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
    private readonly Lazy<int> _equivalencyHash;

    public ArrayType(uint count, IType elementType)
    {
        _count = count;
        ElementType = elementType;
        _equivalencyHash = new(() => HashCode.Combine(ElementType.GetEquivalencyHash(), _count));
    }

    public Bytes Size => ElementType.Size * _count;
    public IEnumerable<IType> Types => Enumerable.Repeat(ElementType, (int) _count);
    public IEnumerable<Bytes> Offsets => Enumerable.Range(0, (int) _count).Select(i => ElementType.Size * (uint) i);

    public IType ElementType { get; }

    public IType GetType(ISpace space, IExpression index)
    {
        return ElementType;
    }

    public IExpression GetOffsetBits(IExpressionFactory exprFactory, ISpace space, IExpression index)
    {
        return GetOffset(exprFactory, index, (uint) ElementType.Size.ToBits());
    }

    public IExpression GetOffsetBytes(IExpressionFactory exprFactory, ISpace space, IExpression index)
    {
        return GetOffset(exprFactory, index, (uint) ElementType.Size);
    }

    public IType Resize(Bytes allocatedSize)
    {
        // We've got an array here so need to replicate the element type n times
        if ((uint) allocatedSize % (uint) ElementType.Size != 0)
            throw new Exception("Expected the type to be an array element, but doesnt seem to fit the allocation.");

        var elements = (uint) allocatedSize / (uint) ElementType.Size;
        return new ArrayType(elements, ElementType);
    }

    private static IExpression GetOffset(IExpressionFactory exprFactory, IExpression index, BigInteger elementSize)
    {
        return exprFactory.CreateConstant(exprFactory.PointerSize, elementSize).Multiply(index.SignExtend(exprFactory.PointerSize));
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IType other)
    {
        return other is ArrayType at
            ? ElementType.IsEquivalentTo(at.ElementType)
                .And((new(), _count == at._count))
            : (new(), false);
    }

    public object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Count = _count,
            ElementType = ElementType.ToJson()
        };
    }

    public int GetEquivalencyHash()
    {
        return _equivalencyHash.Value;
    }
}
