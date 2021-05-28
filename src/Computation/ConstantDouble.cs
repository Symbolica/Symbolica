using System;
using System.Numerics;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class ConstantDouble : ConstantFloat
    {
        private readonly double _value;

        public ConstantDouble(double value)
            : base((Bits) 64U)
        {
            _value = value;
        }

        public override IValue FloatAdd(IValue value)
        {
            return Create(value, (l, r) => l.FloatAdd(r), (l, r) => l + r);
        }

        public override IValue FloatCeiling()
        {
            return new ConstantDouble(Math.Ceiling(_value));
        }

        public override IValue FloatConvert(Bits size)
        {
            return (uint) size switch
            {
                32U => new ConstantSingle((float) _value),
                64U => this,
                _ => ToSymbolicFloat().FloatConvert(size)
            };
        }

        public override IValue FloatDivide(IValue value)
        {
            return Create(value, (l, r) => l.FloatDivide(r), (l, r) => l / r);
        }

        public override IValue FloatEqual(IValue value)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return Create(value, (l, r) => l.FloatEqual(r), (l, r) => l == r);
        }

        public override IValue FloatFloor()
        {
            return new ConstantDouble(Math.Floor(_value));
        }

        public override IValue FloatGreater(IValue value)
        {
            return Create(value, (l, r) => l.FloatGreater(r), (l, r) => l > r);
        }

        public override IValue FloatGreaterOrEqual(IValue value)
        {
            return Create(value, (l, r) => l.FloatGreaterOrEqual(r), (l, r) => l >= r);
        }

        public override IValue FloatLess(IValue value)
        {
            return Create(value, (l, r) => l.FloatLess(r), (l, r) => l < r);
        }

        public override IValue FloatLessOrEqual(IValue value)
        {
            return Create(value, (l, r) => l.FloatLessOrEqual(r), (l, r) => l <= r);
        }

        public override IValue FloatMultiply(IValue value)
        {
            return Create(value, (l, r) => l.FloatMultiply(r), (l, r) => l * r);
        }

        public override IValue FloatNegate()
        {
            return new ConstantDouble(-_value);
        }

        public override IValue FloatNotEqual(IValue value)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return Create(value, (l, r) => l.FloatNotEqual(r), (l, r) => l != r);
        }

        public override IValue FloatOrdered(IValue value)
        {
            return Create(value, (l, r) => l.FloatOrdered(r), (l, r) => !(double.IsNaN(l) || double.IsNaN(r)));
        }

        public override IValue FloatRemainder(IValue value)
        {
            return Create(value, (l, r) => l.FloatRemainder(r), Math.IEEERemainder);
        }

        public override IValue FloatSubtract(IValue value)
        {
            return Create(value, (l, r) => l.FloatSubtract(r), (l, r) => l - r);
        }

        public override IValue FloatToSigned(Bits size)
        {
            return ConstantUnsigned.Create(size, (BigInteger) _value);
        }

        public override IValue FloatToUnsigned(Bits size)
        {
            return ConstantUnsigned.Create(size, (BigInteger) _value);
        }

        public override IValue FloatUnordered(IValue value)
        {
            return Create(value, (l, r) => l.FloatUnordered(r), (l, r) => double.IsNaN(l) || double.IsNaN(r));
        }

        public override SymbolicFloat ToSymbolicFloat()
        {
            return new(Size, c => c.MkFP(_value, SymbolicFloat.Sort(c, Size)));
        }

        public override ConstantSigned ToConstantSigned()
        {
            return ConstantSigned.Create(Size, BitConverter.DoubleToInt64Bits(_value));
        }

        private IValue Create(IValue other,
            Func<SymbolicFloat, IValue, IValue> symbolic,
            Func<double, double, double> func)
        {
            return Create(other, symbolic, func, c => new ConstantDouble(c));
        }

        private IValue Create(IValue other,
            Func<SymbolicFloat, IValue, IValue> symbolic,
            Func<double, double, bool> func)
        {
            return Create(other, symbolic, func, c => new ConstantBool(c));
        }

        private IValue Create<TConstant>(IValue other,
            Func<SymbolicFloat, IValue, IValue> symbolic,
            Func<double, double, TConstant> func,
            Func<TConstant, IValue> constructor)
        {
            return other is IConstantValue c && c.ToConstantFloat() is ConstantDouble cd
                ? constructor(func(_value, cd._value))
                : symbolic(ToSymbolicFloat(), other);
        }
    }
}
