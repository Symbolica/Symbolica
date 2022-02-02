using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal static class Read
{
    public static IValue Create(ICollectionFactory collectionFactory, IValue buffer, IValue offset, Bits size)
    {
        return Value.Create(buffer, offset,
            (b, o) => b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), size),
            (b, o) => b is Write w
                ? w.Read(collectionFactory, o, size)
                : Truncate.Create(size, LogicalShiftRight.Create(b, o)));
    }
}
