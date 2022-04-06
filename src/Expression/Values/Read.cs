

using Symbolica.Collection;

namespace Symbolica.Expression.Values;

public static class Read
{
    public static IExpression<IType> Create(ICollectionFactory collectionFactory,
        IExpression<IType> buffer, IExpression<IType> offset, Bits size)
    {
        return buffer is IConstantValue<IType> b && offset is IConstantValue<IType> o
            ? b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), size)
            : buffer is Write w
                ? w.LayerRead(collectionFactory, offset, size)
                : Truncate.Create(
                    size,
                    LogicalShiftRight.Create(buffer, Resize.Create(buffer.Size, offset)));
    }
}
