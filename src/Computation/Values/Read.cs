using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal static class Read
{
    public static IValue Create(ICollectionFactory collectionFactory, ISolver solver,
        IValue buffer, IValue offset, Bits size)
    {
        return buffer is IConstantValue b && offset is IConstantValue o
            ? b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), size)
            : buffer is Write w
                ? w.LayerRead(collectionFactory, solver, offset, size)
                : SymbolicRead(solver, buffer, offset, size);
    }

    private static IValue SymbolicRead(ISolver solver, IValue buffer, IValue offset, Bits size)
    {
        var value = Truncate.Create(size, LogicalShiftRight.Create(buffer, offset));

        return solver.TryGetSingleValue(value, out var constant)
            ? ConstantUnsigned.Create(value.Size, constant)
            : value;
    }
}
