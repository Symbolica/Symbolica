using System.Numerics;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicWriteExpression : SymbolicExpression
    {
        private readonly IExpression _writeBuffer;
        private readonly IExpression _writeOffset;
        private readonly IExpression _writeValue;

        public SymbolicWriteExpression(IContextFactory contextFactory, ICollectionFactory collectionFactory,
            IExpression writeBuffer, IExpression writeOffset, IExpression writeValue)
            : base(contextFactory, collectionFactory)
        {
            _writeBuffer = writeBuffer;
            _writeOffset = writeOffset;
            _writeValue = writeValue;
        }

        public override Bits Size => _writeBuffer.Size;
        public override IValue Value => ((IValueExpression) Flatten()).Value;
        public override IValue[] Constraints => ((IValueExpression) Flatten()).Constraints;

        public override IExpression Read(IExpression offset, Bits size)
        {
            var readMask = Mask(offset, size);
            var writeMask = Mask(_writeOffset, _writeValue.Size);

            return readMask is ConstantExpression && writeMask is ConstantExpression
                ? readMask.And(writeMask).Constant.IsZero
                    ? _writeBuffer.Read(offset, size)
                    : readMask.Xor(writeMask).Constant.IsZero
                        ? _writeValue
                        : Flatten().Read(offset, size)
                : Flatten().Read(offset, size);
        }

        private IExpression Flatten()
        {
            var writeMask = Mask(_writeOffset, _writeValue.Size);

            return _writeBuffer.And(writeMask.Not()).Or(_writeValue.ZeroExtend(Size).ShiftLeft(_writeOffset));
        }

        private IExpression Mask(IExpression offset, Bits size)
        {
            var zero = new ConstantExpression(ContextFactory, CollectionFactory,
                ConstantUnsigned.Create(size, BigInteger.Zero));

            return zero.Not().ZeroExtend(Size).ShiftLeft(offset);
        }
    }
}
