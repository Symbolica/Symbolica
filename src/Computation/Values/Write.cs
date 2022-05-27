using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal static class Write
{
    public static IValue Create(ICollectionFactory collectionFactory, IValue buffer, IValue offset, IValue value)
    {
        return buffer is IConstantValue b && offset is IConstantValue o && value is IConstantValue v
            ? b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory))
            : Or.Create(
                And.Create(
                    buffer,
                    Not.Create(
                        ShiftLeft.Create(ConstantUnsigned.CreateZero(value.Size).Not().Extend(buffer.Size), offset))),
                ShiftLeft.Create(ZeroExtend.Create(buffer.Size, value), offset));
    }
}
