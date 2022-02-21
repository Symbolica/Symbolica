using System.Numerics;
using System.Threading;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal static class Read
{
    public static IValue Create(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue buffer, IValue offset, Bits size)
    {
        IValue NonZeroSymbolicOffsetRead()
        {
            return Truncate.Create(size, LogicalShiftRight.Create(buffer, offset));
        }
        return buffer is IConstantValue b
            ? offset is IConstantValue o
                ? b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), size)
                : ((BigInteger) b.AsUnsigned()).IsZero
                    ? ConstantUnsigned.Create(size, BigInteger.Zero)
                    : NonZeroSymbolicOffsetRead()
            : buffer is Write w
                ? w.LayerRead(collectionFactory, assertions, offset, size)
                : Truncate.Create(size, LogicalShiftRight.Create(buffer, offset));
    }
}
