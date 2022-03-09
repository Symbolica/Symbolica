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
            WriteOffsets.Create(offset, buffer.Size, size),
            size);
    }

    internal static IValue Create(ICollectionFactory collectionFactory, IAssertions assertions,
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
                ? b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), offset.FieldSize)
                : SymbolicRead(assertions, buffer, offset.Value, offset.FieldSize);

            // if (subBuffer is Truncate && buffer is not IConstantValue)
            //     Debugger.Break();
            return Create(collectionFactory, assertions, subBuffer, offsets.Tail(), size);
        }

        return buffer is AggregateWrite w
            ? w.Read(collectionFactory, assertions, offsets, size)
            : ReadNonAggregateWrite();
    }

    private static IValue SymbolicRead(IAssertions assertions, IValue buffer, IValue offset, Bits size)
    {
        var value = Truncate.Create(
            size,
            LogicalShiftRight.Create(
                buffer,
                Truncate.Create(buffer.Size, ZeroExtend.Create(buffer.Size, offset))));

        var constant = assertions.GetConstant(value);
        using var proposition = assertions.GetProposition(Equal.Create(constant, value));
        return proposition.CanBeFalse ? value : constant;
    }
}
