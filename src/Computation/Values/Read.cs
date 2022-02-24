using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal static class Read
{
    public static IValue Create(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue buffer, IValue offset, Bits size)
    {
        if (offset is AggregateOffset ao)
        {
            if (!ao.IsBounded(assertions, size))
            {
                return Create(collectionFactory, assertions, buffer, ao.Aggregate(), size);
            }

            return buffer is AggregateWrite aw
                ? aw.Read(collectionFactory, assertions, ao, size)
                : buffer is Write w1
                    ? w1.ReadAggregate(collectionFactory, assertions, ao, size)
                    : Create(collectionFactory, assertions, buffer, ao.Aggregate(), size);
        }
        return buffer is IConstantValue b && offset is IConstantValue o
            ? b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), size)
            : buffer is Write w
                ? w.LayerRead(collectionFactory, assertions, offset, size)
                : Truncate.Create(
                    size,
                    LogicalShiftRight.Create(
                        buffer,
                        ZeroExtend.Create(buffer.Size, offset)));
    }
}
