using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal static class Read
{
    public static IValue Create(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue buffer, IValue offset, Bits size)
    {
        IValue UniversalRead(IValue offset)
        {
            return Truncate.Create(
                size,
                LogicalShiftRight.Create(buffer, ZeroExtend.Create(buffer.Size, offset)));
        }
        // TODO: See comments in Write.Create
        if (offset is AggregateOffset ao && ao.IsBounded(assertions, size))
        {
            return buffer is AggregateWrite aw
                ? aw.Read(collectionFactory, assertions, ao, size)
                : buffer is Write w1
                    ? w1.ReadAggregate(collectionFactory, assertions, ao, size)
                    : UniversalRead(ao.Aggregate());
        }
        return buffer is IConstantValue b && offset is IConstantValue o
            ? b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), size)
            : buffer is Write w
                ? w.LayerRead(collectionFactory, assertions, offset, size)
                : UniversalRead(offset);
    }
}
