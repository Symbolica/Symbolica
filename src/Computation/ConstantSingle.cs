using System;
using System.Numerics;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class ConstantSingle : ConstantFloat
    {
        private readonly float _value;

        public ConstantSingle(float value)
            : base((Bits) 32U)
        {
            _value = value;
        }

        public override IValue FloatAdd(IValue value)
        {
            return Create(value, (l, r) => l.FloatAdd(r), (l, r) => l + r);
        }

        public override IValue FloatCeiling()
        {
            return new ConstantSingle(MathF.Ceiling(_value));
        }

        public override IValue FloatConvert(Bits size)
        {
            return (uint) size switch
            {
                32U => this,
                64U => new ConstantDouble(_value),
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
            return new ConstantSingle(MathF.Floor(_value));
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
            return new ConstantSingle(-_value);
        }

        public override IValue FloatNotEqual(IValue value)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return Create(value, (l, r) => l.FloatNotEqual(r), (l, r) => l != r);
        }

        public override IValue FloatOrdered(IValue value)
        {
            return Create(value, (l, r) => l.FloatOrdered(r), (l, r) => !(float.IsNaN(l) || float.IsNaN(r)));
        }

        public override IValue FloatRemainder(IValue value)
        {
            return Create(value, (l, r) => l.FloatRemainder(r), MathF.IEEERemainder);
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
            return Create(value, (l, r) => l.FloatUnordered(r), (l, r) => float.IsNaN(l) || float.IsNaN(r));
        }

        public override SymbolicFloat ToSymbolicFloat()
        {
            return new(Size, c => c.MkFP(_value, Size.GetSort(c)));
        }

        public override ConstantSigned ToConstantSigned()
        {
            return ConstantSigned.Create(Size, BitConverter.SingleToInt32Bits(_value));
        }

        private IValue Create(IValue other,
            Func<SymbolicFloat, IValue, IValue> symbolic,
            Func<float, float, float> func)
        {
            return Create(other, symbolic, func, c => new ConstantSingle(c));
        }

        private IValue Create(IValue other,
            Func<SymbolicFloat, IValue, IValue> symbolic,
            Func<float, float, bool> func)
        {
            return Create(other, symbolic, func, c => new ConstantBool(c));
        }

        private IValue Create<TConstant>(IValue other,
            Func<SymbolicFloat, IValue, IValue> symbolic,
            Func<float, float, TConstant> func,
            Func<TConstant, IValue> constructor)
        {
            return other is IConstantValue c && c.ToConstantFloat() is ConstantSingle cs
                ? constructor(func(_value, cs._value))
                : symbolic(ToSymbolicFloat(), other);
        }
    }
}
