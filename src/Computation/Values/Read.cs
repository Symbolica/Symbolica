using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal static class Read
{
    public static IValue Create(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue buffer, IValue offset, Bits size)
    {
        return Create(
            collectionFactory,
            assertions,
            buffer,
            WriteOffsets.Create(assertions, offset, buffer.Size, size),
            size);
    }

    private static IValue Create(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue buffer, WriteOffsets offsets, Bits size)
    {
        IValue ReadNonAggregateWrite()
        {
            if (offsets.Empty)
            {
                if (buffer.Size != size)
                    throw new InconsistentExpressionSizesException(buffer.Size, size);

                return buffer;
            }

            var offset = offsets.Head();
            var subBuffer = buffer is IConstantValue b && offset.Value is IConstantValue o
                ? b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), size)
                : Truncate.Create(
                    size,
                    LogicalShiftRight.Create(
                        buffer,
                        ZeroExtend.Create(buffer.Size, offset.Value)));
            return Create(collectionFactory, assertions, subBuffer, offsets.Tail(), size);
        }

        return buffer is AggregateWrite w
            ? w.Read(collectionFactory, assertions, offsets, size)
            : ReadNonAggregateWrite();
    }
}
