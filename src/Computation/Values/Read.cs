using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal static class Read
{
    public static IValue Create(ICollectionFactory collectionFactory, ISolver solver,
        IValue buffer, IValue offset, Bits size)
    {
        IValue UniversalRead(IValue offset)
        {
            return Truncate.Create(
                size,
                LogicalShiftRight.Create(buffer, ZeroExtend.Create(buffer.Size, offset)));
        }
        // TODO: See comments in Write.Create
        if (offset is AggregateOffset ao && ao.IsBounded(solver, size))
        {
            return buffer is AggregateWrite aw
                ? aw.Read(collectionFactory, solver, ao, size)
                : buffer is Write w1
                    ? w1.ReadAggregate(collectionFactory, solver, ao, size)
                    : UniversalRead(ao.Aggregate());
        }
        return buffer is IConstantValue b && offset is IConstantValue o
            ? b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), size)
            : buffer is Write w
                ? w.LayerRead(collectionFactory, solver, offset, size)
                : UniversalRead(offset);
    }
}
