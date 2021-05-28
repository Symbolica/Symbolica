using System;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class ConstantBitVector : ConstantInteger
    {
        private readonly IPersistentList<byte> _value;

        private ConstantBitVector(Bits size, IPersistentList<byte> value)
            : base(size)
        {
            _value = value;
        }

        public override BigInteger Integer => ToConstantUnsigned().Integer;

        public override IProposition GetProposition(IPersistentSpace space, SymbolicBool[] constraints)
        {
            return ToConstantBool().GetProposition(space, constraints);
        }

        public override IValue Add(IValue value)
        {
            return ToConstantUnsigned().Add(value);
        }

        public override IValue And(IValue value)
        {
            return ToConstantUnsigned().And(value);
        }

        public override IValue ArithmeticShiftRight(IValue value)
        {
            return ToConstantSigned().ArithmeticShiftRight(value);
        }

        public override IValue Equal(IValue value)
        {
            return ToConstantUnsigned().Equal(value);
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
            return ToConstantUnsigned().Not();
        }

        public override IValue NotEqual(IValue value)
        {
            return ToConstantUnsigned().NotEqual(value);
        }

        public override IValue Or(IValue value)
        {
            return ToConstantUnsigned().Or(value);
        }

        public override IValue Read(ICollectionFactory collectionFactory, IValue offset, Bits size)
        {
            return Size == offset.Size
                ? offset is IConstantValue co
                    ? new ConstantBitVector(size, _value.GetRange(GetIndex(co), GetCount(size)))
                    : ToSymbolicBitVector().Read(collectionFactory, offset, size)
                : throw new Exception("Expression sizes are different.");
        }

        public override IValue Select(IValue trueValue, IValue falseValue)
        {
            return ToConstantBool().Select(trueValue, falseValue);
        }

        public override IValue Select(Func<Context, BoolExpr> predicate, IValue falseValue)
        {
            return ToSymbolicBitVector().Select(predicate, falseValue);
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
            return Size == offset.Size
                ? offset is IConstantValue co && value is IConstantValue cv
                    ? new ConstantBitVector(Size, _value.SetRange(GetIndex(co), GetBytes(collectionFactory, cv)))
                    : ToSymbolicBitVector().Write(collectionFactory, offset, value)
                : throw new Exception("Expression sizes are different.");
        }

        public override IValue Xor(IValue value)
        {
            return ToConstantUnsigned().Xor(value);
        }

        public override IValue ZeroExtend(Bits size)
        {
            return ToConstantUnsigned().ZeroExtend(size);
        }

        public override SymbolicBitVector ToSymbolicBitVector()
        {
            return ToConstantUnsigned().ToSymbolicBitVector();
        }

        public override SymbolicBool ToSymbolicBool()
        {
            return ToConstantUnsigned().ToSymbolicBool();
        }

        public override ConstantBitVector ToConstantBitVector(ICollectionFactory collectionFactory)
        {
            return this;
        }

        public override ConstantUnsigned ToConstantUnsigned()
        {
            return ConstantUnsigned.Create(Size, new BigInteger(_value.ToArray(), true));
        }

        public override ConstantSigned ToConstantSigned()
        {
            return ToConstantUnsigned().ToConstantSigned();
        }

        public override ConstantBool ToConstantBool()
        {
            return ToConstantUnsigned().ToConstantBool();
        }

        public override IValue ToConstantFloat()
        {
            return ToConstantSigned().ToConstantFloat();
        }

        public static ConstantBitVector Create(ICollectionFactory collectionFactory, Bits size, BigInteger value)
        {
            var bytes = new byte[GetCount(size)];
            value.TryWriteBytes(bytes, out _, true);

            return new ConstantBitVector(size, collectionFactory.CreatePersistentList<byte>().AddRange(bytes));
        }

        private static int GetIndex(IConstantValue offset)
        {
            return (int) (uint) ((Bits) (uint) offset.ToConstantUnsigned().Integer).ToBytes();
        }

        private static int GetCount(Bits size)
        {
            return (int) (uint) size.ToBytes();
        }

        private static IPersistentList<byte> GetBytes(ICollectionFactory collectionFactory, IConstantValue value)
        {
            return value.ToConstantBitVector(collectionFactory)._value;
        }
    }
}
