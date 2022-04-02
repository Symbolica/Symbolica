

using Symbolica.Collection;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public static class Read
{
    public static IExpression<IType> Create(ICollectionFactory collectionFactory, ISpace space,
        IExpression<IType> buffer, IExpression<IType> offset, Bits size)
    {
        if (offset is Address a)
            offset = a.Aggregate();

        return buffer is IConstantValue<IType> b && offset is IConstantValue<IType> o
            ? b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), size)
            : buffer is Write w
                ? w.LayerRead(collectionFactory, space, offset, size)
                : SymbolicRead(space, buffer, offset, size);
    }

    private static IExpression<IType> SymbolicRead(ISpace space, IExpression<IType> buffer, IExpression<IType> offset, Bits size)
    {
        var value = Truncate.Create(
            size,
            LogicalShiftRight.Create(buffer, Resize.Create(buffer.Size, offset)));

        return space.TryGetSingleValue(value, out var constant)
            ? ConstantUnsigned.Create(value.Size, constant)
            : value;
    }
}
