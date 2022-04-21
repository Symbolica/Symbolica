using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal static class Read
{
    public static IValue Create(ICollectionFactory collectionFactory, IValue buffer, IValue offset, Bits size)
    {
        return buffer is IConstantValue b && offset is IConstantValue o
            ? b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), size)
            : Truncate.Create(size, LogicalShiftRight.Create(buffer, offset));
    }
}
