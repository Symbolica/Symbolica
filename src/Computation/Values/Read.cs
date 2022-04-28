using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal static class Read
{
    public static IValue Create(ICollectionFactory collectionFactory, IPersistentSpace space,
        IValue buffer, IValue offset, Bits size)
    {
        return buffer is IConstantValue b && offset is IConstantValue o
            ? b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), size)
            : buffer is Write w
                ? w.LayerRead(collectionFactory, space, offset, size)
                : TryGetSingleValue(space, Truncate.Create(size, LogicalShiftRight.Create(buffer, offset)));
    }

    private static IValue TryGetSingleValue(IPersistentSpace space, IValue value)
    {
        using var solver = space.CreateSolver();

        var temp = solver.TryGetSingleValue(value);

        return temp.HasValue
            ? ConstantUnsigned.Create(value.Size, temp.Value)
            : value;
    }
}
