using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal static class Read
{
    public static IValue Create(ICollectionFactory collectionFactory, ISolver solver,
        IValue buffer, IValue offset, Bits size)
    {
        return Create(
            collectionFactory,
            solver,
            buffer,
            WriteOffsets.Create(offset, buffer.Size, size),
            size);
    }

    internal static IValue Create(ICollectionFactory collectionFactory, ISolver solver,
        IValue buffer, WriteOffsets offsets, Bits size)
    {
        if (buffer is Write w)
            return w.LayerRead(collectionFactory, solver, offsets, size);

        if (offsets.Empty)
        {
            if (buffer.Size != size)
                throw new InconsistentExpressionSizesException(buffer.Size, size);

            return buffer;
        }

        var offset = offsets.Head();
        var subBuffer = buffer is IConstantValue b && offset.Value is IConstantValue o
            ? b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), offset.FieldSize)
            : SymbolicRead(solver, buffer, offset.Value, offset.FieldSize);
        return Create(collectionFactory, solver, subBuffer, offsets.Tail(), size);
    }

    private static IValue SymbolicRead(ISolver solver, IValue buffer, IValue offset, Bits size)
    {
        var value = Truncate.Create(
            size,
            LogicalShiftRight.Create(buffer, Resize.Create(buffer.Size, offset)));

        return solver.TryGetSingleValue(value, out var constant)
            ? ConstantUnsigned.Create(size, constant)
            : value;
    }
}
