using Symbolica.Collection;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record Write : IBitVectorExpression
{
    private readonly IExpression<IType> _writeBuffer;
    private readonly IExpression<IType> _writeMask;
    private readonly IExpression<IType> _writeOffset;
    private readonly IExpression<IType> _writeValue;

    private Write(IExpression<IType> writeBuffer, IExpression<IType> writeOffset, IExpression<IType> writeValue)
    {
        _writeBuffer = writeBuffer;
        _writeOffset = writeOffset;
        _writeValue = writeValue;
        _writeMask = Mask(writeBuffer, writeOffset, writeValue.Size);
        Type = new BitVector(writeBuffer.Size);
    }

    public BitVector Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as Write);
    }

    public U Map<U>(IArityMapper<U> mapper)
    {
        return mapper.Map(this);
    }

    public U Map<U>(ITypeMapper<U> mapper)
    {
        return mapper.Map(this);
    }

    public U Map<U>(IIntegerMapper<U> mapper)
    {
        return mapper.Map(this);
    }

    public U Map<U>(IBitVectorMapper<U> mapper)
    {
        return mapper.Map(this);
    }

    public IExpression<IType> Flatten()
    {
        var writeData = ShiftLeft.Create(
            ZeroExtend.Create(Type.Size, _writeValue),
            Resize.Create(Type.Size, _writeOffset));

        return Or.Create(And.Create(_writeBuffer, Not.Create(_writeMask)), writeData);
    }

    internal IExpression<IType> LayerRead(ICollectionFactory collectionFactory, IExpression<IType> offset, Bits size)
    {
        var mask = Mask(this, offset, size);

        return IsNotOverlapping(mask)
            ? Read.Create(collectionFactory, _writeBuffer, offset, size)
            : IsAligned(mask)
                ? _writeValue
                : Read.Create(collectionFactory, Flatten(), offset, size);
    }

    private IExpression<IType> LayerWrite(ICollectionFactory collectionFactory, IExpression<IType> offset, IExpression<IType> value)
    {
        var mask = Mask(this, offset, value.Size);

        return IsNotOverlapping(mask)
            ? new Write(Create(collectionFactory, _writeBuffer, offset, value), _writeOffset, _writeValue)
            : IsAligned(mask)
                ? new Write(_writeBuffer, offset, value)
                : new Write(this, offset, value);
    }

    private bool IsNotOverlapping(IExpression<IType> mask)
    {
        return And.Create(mask, _writeMask) is IConstantValue<IType> a && a.AsUnsigned().IsZero;
    }

    private bool IsAligned(IExpression<IType> mask)
    {
        return Xor.Create(mask, _writeMask) is IConstantValue<IType> x && x.AsUnsigned().IsZero;
    }

    private static IExpression<IType> Mask(IExpression<IType> buffer, IExpression<IType> offset, Bits size)
    {
        return ShiftLeft.Create(
            ConstantUnsigned.CreateZero(size).Not().Extend(buffer.Size),
            Resize.Create(buffer.Size, offset));
    }

    public static IExpression<IType> Create(ICollectionFactory collectionFactory,
        IExpression<IType> buffer, IExpression<IType> offset, IExpression<IType> value)
    {
        return buffer is IConstantValue<IType> b && offset is IConstantValue<IType> o && value is IConstantValue<IType> v
            ? b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory))
            : buffer is Write w
                ? w.LayerWrite(collectionFactory, offset, value)
                : new Write(buffer, offset, value);
    }
}
