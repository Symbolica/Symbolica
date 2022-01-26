using System;
using System.Numerics;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicWriteExpression : IValueExpression
    {
        private readonly ICollectionFactory _collectionFactory;
        private readonly IContextFactory _contextFactory;
        private readonly IValueExpression _writeBuffer;
        private readonly IExpression _writeOffset;
        private readonly IExpression _writeValue;

        internal SymbolicWriteExpression(
            IContextFactory contextFactory,
            ICollectionFactory collectionFactory,
            IValueExpression writeBuffer,
            IExpression writeOffset,
            IExpression writeValue)
        {
            _contextFactory = contextFactory;
            _collectionFactory = collectionFactory;
            _writeBuffer = writeBuffer;
            _writeOffset = writeOffset;
            _writeValue = writeValue;
        }

        public Bits Size => Value.Size;
        public BigInteger Constant => AsSymbolic().Constant;
        public IValue Value => AsSymbolic().Value;
        public IValue[] Constraints => AsSymbolic().Constraints;

        public IExpression GetValue(ISpace space)
        {
            return AsSymbolic().GetValue(space);
        }

        public IProposition GetProposition(ISpace space)
        {
            return AsSymbolic().GetProposition(space);
        }

        public IExpression Add(IExpression expression)
        {
            return AsSymbolic().Add(expression);
        }

        public IExpression And(IExpression expression)
        {
            return AsSymbolic().And(expression);
        }

        public IExpression ArithmeticShiftRight(IExpression expression)
        {
            return AsSymbolic().ArithmeticShiftRight(expression);
        }

        public IExpression Equal(IExpression expression)
        {
            return AsSymbolic().Equal(expression);
        }

        public IExpression FloatAdd(IExpression expression)
        {
            return AsSymbolic().FloatAdd(expression);
        }

        public IExpression FloatCeiling()
        {
            return AsSymbolic().FloatCeiling();
        }

        public IExpression FloatConvert(Bits size)
        {
            return AsSymbolic().FloatConvert(size);
        }

        public IExpression FloatDivide(IExpression expression)
        {
            return AsSymbolic().FloatDivide(expression);
        }

        public IExpression FloatEqual(IExpression expression)
        {
            return AsSymbolic().FloatEqual(expression);
        }

        public IExpression FloatFloor()
        {
            return AsSymbolic().FloatFloor();
        }

        public IExpression FloatGreater(IExpression expression)
        {
            return AsSymbolic().FloatGreater(expression);
        }

        public IExpression FloatGreaterOrEqual(IExpression expression)
        {
            return AsSymbolic().FloatGreaterOrEqual(expression);
        }

        public IExpression FloatLess(IExpression expression)
        {
            return AsSymbolic().FloatLess(expression);
        }

        public IExpression FloatLessOrEqual(IExpression expression)
        {
            return AsSymbolic().FloatLessOrEqual(expression);
        }

        public IExpression FloatMultiply(IExpression expression)
        {
            return AsSymbolic().FloatMultiply(expression);
        }

        public IExpression FloatNegate()
        {
            return AsSymbolic().FloatNegate();
        }

        public IExpression FloatNotEqual(IExpression expression)
        {
            return AsSymbolic().FloatNotEqual(expression);
        }

        public IExpression FloatOrdered(IExpression expression)
        {
            return AsSymbolic().FloatOrdered(expression);
        }

        public IExpression FloatPower(IExpression expression)
        {
            return AsSymbolic().FloatPower(expression);
        }

        public IExpression FloatRemainder(IExpression expression)
        {
            return AsSymbolic().FloatRemainder(expression);
        }

        public IExpression FloatSubtract(IExpression expression)
        {
            return AsSymbolic().FloatSubtract(expression);
        }

        public IExpression FloatToSigned(Bits size)
        {
            return AsSymbolic().FloatToSigned(size);
        }

        public IExpression FloatToUnsigned(Bits size)
        {
            return AsSymbolic().FloatToUnsigned(size);
        }

        public IExpression FloatUnordered(IExpression expression)
        {
            return AsSymbolic().FloatUnordered(expression);
        }

        public IExpression LogicalShiftRight(IExpression expression)
        {
            return AsSymbolic().LogicalShiftRight(expression);
        }

        public IExpression Multiply(IExpression expression)
        {
            return AsSymbolic().Multiply(expression);
        }

        public IExpression Not()
        {
            return AsSymbolic().Not();
        }

        public IExpression NotEqual(IExpression expression)
        {
            return AsSymbolic().NotEqual(expression);
        }

        public IExpression Or(IExpression expression)
        {
            return AsSymbolic().Or(expression);
        }

        public IExpression Read(IExpression offset, Bits size)
        {
            var readMask = Mask(this, offset, size);
            var writeMask = Mask(_writeBuffer, _writeOffset, _writeValue.Size);

            return readMask is ConstantExpression && writeMask is ConstantExpression
                ? readMask.And(writeMask).Constant.IsZero
                    ? _writeBuffer.Read(offset, size)
                    : readMask.Xor(writeMask).Constant.IsZero
                        ? _writeValue
                        : AsSymbolic().Read(offset, size)
                : AsSymbolic().Read(offset, size);
        }

        public IExpression Select(IExpression trueValue, IExpression falseValue)
        {
            return AsSymbolic().Select(trueValue, falseValue);
        }

        public IExpression ShiftLeft(IExpression expression)
        {
            return AsSymbolic().ShiftLeft(expression);
        }

        public IExpression SignedDivide(IExpression expression)
        {
            return AsSymbolic().SignedDivide(expression);
        }

        public IExpression SignedGreater(IExpression expression)
        {
            return AsSymbolic().SignedGreater(expression);
        }

        public IExpression SignedGreaterOrEqual(IExpression expression)
        {
            return AsSymbolic().SignedGreaterOrEqual(expression);
        }

        public IExpression SignedLess(IExpression expression)
        {
            return AsSymbolic().SignedLess(expression);
        }

        public IExpression SignedLessOrEqual(IExpression expression)
        {
            return AsSymbolic().SignedLessOrEqual(expression);
        }

        public IExpression SignedRemainder(IExpression expression)
        {
            return AsSymbolic().SignedRemainder(expression);
        }

        public IExpression SignedToFloat(Bits size)
        {
            return AsSymbolic().SignedToFloat(size);
        }

        public IExpression SignExtend(Bits size)
        {
            return AsSymbolic().SignExtend(size);
        }

        public IExpression Subtract(IExpression expression)
        {
            return AsSymbolic().Subtract(expression);
        }

        public IExpression Truncate(Bits size)
        {
            return AsSymbolic().Truncate(size);
        }

        public IExpression UnsignedDivide(IExpression expression)
        {
            return AsSymbolic().UnsignedDivide(expression);
        }

        public IExpression UnsignedGreater(IExpression expression)
        {
            return AsSymbolic().UnsignedGreater(expression);
        }

        public IExpression UnsignedGreaterOrEqual(IExpression expression)
        {
            return AsSymbolic().UnsignedGreaterOrEqual(expression);
        }

        public IExpression UnsignedLess(IExpression expression)
        {
            return AsSymbolic().UnsignedLess(expression);
        }

        public IExpression UnsignedLessOrEqual(IExpression expression)
        {
            return AsSymbolic().UnsignedLessOrEqual(expression);
        }

        public IExpression UnsignedRemainder(IExpression expression)
        {
            return AsSymbolic().UnsignedRemainder(expression);
        }

        public IExpression UnsignedToFloat(Bits size)
        {
            return AsSymbolic().UnsignedToFloat(size);
        }

        public IExpression Write(IExpression offset, IExpression value)
        {
            return AsSymbolic().Write(offset, value);
        }

        public IExpression Xor(IExpression expression)
        {
            return AsSymbolic().Xor(expression);
        }

        public IExpression ZeroExtend(Bits size)
        {
            return AsSymbolic().ZeroExtend(size);
        }

        private IValueExpression AsSymbolic()
        {
            var writeMask = Mask(_writeBuffer, _writeOffset, _writeValue.Size);
            return (IValueExpression) _writeBuffer.And(writeMask.Not()).Or(_writeValue.ZeroExtend(_writeBuffer.Size).ShiftLeft(_writeOffset));
        }

        private IExpression Mask(
            IValueExpression buffer,
            IExpression offset,
            Bits size)
        {
            var zero = new ConstantExpression(_contextFactory, _collectionFactory,
                ConstantUnsigned.Create(size, BigInteger.Zero));

            return zero.Not().ZeroExtend(buffer.Size).ShiftLeft(offset);
        }
    }
}
