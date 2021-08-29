using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class ConstantBool : ConstantInteger
    {
        private readonly bool _value;

        public ConstantBool(bool value)
            : base(Bits.One)
        {
            _value = value;
        }

        public override BigInteger Integer => _value ? BigInteger.One : BigInteger.Zero;

        public override IProposition GetProposition(IPersistentSpace space, SymbolicBool[] constraints)
        {
            return new ConstantProposition(space, _value);
        }

        public override IValue Add(IValue value)
        {
            return ToConstantUnsigned().Add(value);
        }

        public override IValue And(IValue value)
        {
            return Create(value, (l, r) => l.And(r), (l, r) => l && r);
        }

        public override IValue ArithmeticShiftRight(IValue value)
        {
            return ToConstantSigned().ArithmeticShiftRight(value);
        }

        public override IValue Equal(IValue value)
        {
            return Create(value, (l, r) => l.Equal(r), (l, r) => l == r);
        }

        public override IValue LogicalShiftRight(IValue value)
        {
            return ToConstantUnsigned().LogicalShiftRight(value);
        }

        public override IValue Multiply(IValue value)
        {
            return ToConstantUnsigned().Multiply(value);
        }

        public override IValue Not()
        {
            return new ConstantBool(!_value);
        }

        public override IValue NotEqual(IValue value)
        {
            return Create(value, (l, r) => l.NotEqual(r), (l, r) => l != r);
        }

        public override IValue Or(IValue value)
        {
            return Create(value, (l, r) => l.Or(r), (l, r) => l || r);
        }

        public override IValue Read(ICollectionFactory collectionFactory, IValue offset, Bits size)
        {
            return ToConstantBitVector(collectionFactory).Read(collectionFactory, offset, size);
        }

        public override IValue Select(IValue trueValue, IValue falseValue)
        {
            return _value ? trueValue : falseValue;
        }

        public override IValue Select(Func<Context, BoolExpr> predicate, IValue falseValue)
        {
            return ToSymbolicBool().Select(predicate, falseValue);
        }

        public override IValue ShiftLeft(IValue value)
        {
            return ToConstantUnsigned().ShiftLeft(value);
        }

        public override IValue SignedDivide(IValue value)
        {
            return ToConstantSigned().SignedDivide(value);
        }

        public override IValue SignedGreater(IValue value)
        {
            return ToConstantSigned().SignedGreater(value);
        }

        public override IValue SignedGreaterOrEqual(IValue value)
        {
            return ToConstantSigned().SignedGreaterOrEqual(value);
        }

        public override IValue SignedLess(IValue value)
        {
            return ToConstantSigned().SignedLess(value);
        }

        public override IValue SignedLessOrEqual(IValue value)
        {
            return ToConstantSigned().SignedLessOrEqual(value);
        }

        public override IValue SignedRemainder(IValue value)
        {
            return ToConstantSigned().SignedRemainder(value);
        }

        public override IValue SignedToFloat(Bits size)
        {
            return ToConstantSigned().SignedToFloat(size);
        }

        public override IValue SignExtend(Bits size)
        {
            return ToConstantSigned().SignExtend(size);
        }

        public override IValue Subtract(IValue value)
        {
            return ToConstantUnsigned().Subtract(value);
        }

        public override IValue Truncate(Bits size)
        {
            return ToConstantUnsigned().Truncate(size);
        }

        public override IValue UnsignedDivide(IValue value)
        {
            return ToConstantUnsigned().UnsignedDivide(value);
        }

        public override IValue UnsignedGreater(IValue value)
        {
            return ToConstantUnsigned().UnsignedGreater(value);
        }

        public override IValue UnsignedGreaterOrEqual(IValue value)
        {
            return ToConstantUnsigned().UnsignedGreaterOrEqual(value);
        }

        public override IValue UnsignedLess(IValue value)
        {
            return ToConstantUnsigned().UnsignedLess(value);
        }

        public override IValue UnsignedLessOrEqual(IValue value)
        {
            return ToConstantUnsigned().UnsignedLessOrEqual(value);
        }

        public override IValue UnsignedRemainder(IValue value)
        {
            return ToConstantUnsigned().UnsignedRemainder(value);
        }

        public override IValue UnsignedToFloat(Bits size)
        {
            return ToConstantUnsigned().UnsignedToFloat(size);
        }

        public override IValue Write(ICollectionFactory collectionFactory, IValue offset, IValue value)
        {
            return ToConstantBitVector(collectionFactory).Write(collectionFactory, offset, value);
        }

        public override IValue Xor(IValue value)
        {
            return Create(value, (l, r) => l.Xor(r), (l, r) => l ^ r);
        }

        public override IValue ZeroExtend(Bits size)
        {
            return ToConstantUnsigned().ZeroExtend(size);
        }

        public override SymbolicBitVector ToSymbolicBitVector()
        {
            return new(Size, c => c.MkBV(new[] {_value}));
        }

        public override SymbolicBool ToSymbolicBool()
        {
            return new(c => c.MkBool(_value));
        }

        public override ConstantBitVector ToConstantBitVector(ICollectionFactory collectionFactory)
        {
            return ToConstantUnsigned().ToConstantBitVector(collectionFactory);
        }

        public override ConstantUnsigned ToConstantUnsigned()
        {
            return ConstantUnsigned.Create(Size, Integer);
        }

        public override ConstantSigned ToConstantSigned()
        {
            return ConstantSigned.Create(Size, Integer);
        }

        public override ConstantBool ToConstantBool()
        {
            return this;
        }

        public override IValue ToConstantFloat()
        {
            return ToConstantSigned().ToConstantFloat();
        }

        private IValue Create(IValue other,
            Func<SymbolicBool, IValue, IValue> symbolic,
            Func<bool, bool, bool> func)
        {
            return Size == other.Size
                ? other is IConstantValue c
                    ? new ConstantBool(func(_value, c.ToConstantBool()._value))
                    : symbolic(ToSymbolicBool(), other)
                : throw new InconsistentExpressionSizesException(Size, other.Size);
        }
    }
}
