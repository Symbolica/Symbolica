using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal static class Read
{
    public static IValue Create(ICollectionFactory collectionFactory, ISolver solver,
        IValue buffer, IValue offset, Bits size)
    {
        return buffer is IConstantValue b && offset is IConstantValue o
            ? b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), size)
            : buffer is AggregateWrite w
                ? w.Read(
                    collectionFactory,
                    solver,
                    WriteOffsets.Create(solver, offset, buffer.Size, size),
                    size)
                : Truncate.Create(
                    size,
                    LogicalShiftRight.Create(
                        buffer,
                        ZeroExtend.Create(buffer.Size, offset)));
    }
}
