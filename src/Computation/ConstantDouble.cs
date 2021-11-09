using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class ConstantDouble : IFloat, IConstantDouble
    {
        public ConstantDouble(double constant)
        {
            Constant = constant;
        }

        public double Constant { get; }
        public Bits Size => (Bits) 64U;

        public BigInteger AsConstant(IContextFactory contextFactory)
        {
            return AsUnsigned().AsConstant(contextFactory);
        }

        public IValue GetValue(IPersistentSpace space, IBool[] constraints)
        {
            return this;
        }

        public IBitwise AsBitwise()
        {
            return AsUnsigned();
        }

        public IBitVector AsBitVector(ICollectionFactory collectionFactory)
        {
            return AsUnsigned().AsBitVector(collectionFactory);
        }

        public IUnsigned AsUnsigned()
        {
            return AsSigned().AsUnsigned();
        }

        public ISigned AsSigned()
        {
            return ConstantSigned.Create(Size, BitConverter.DoubleToInt64Bits(Constant));
        }

        public IBool AsBool()
        {
            return AsUnsigned().AsBool();
        }

        public IFloat AsFloat()
        {
            return this;
        }

        public IValue IfElse(IBool predicate, IValue falseValue)
        {
            return new SymbolicFloat(Size, Symbolic).IfElse(predicate, falseValue);
        }

        public Func<Context, FPExpr> Symbolic => c => c.MkFP(Constant, Size.GetSort(c));

        public IFloat Add(IFloat value)
        {
            return Create(value, (l, r) => l.Add(r), (l, r) => l + r);
        }

        public IFloat Ceiling()
        {
            return new ConstantDouble(Math.Ceiling(Constant));
        }

        public IFloat Convert(Bits size)
        {
            return (uint) size switch
            {
                32U => new ConstantSingle((float) Constant),
                64U => this,
                _ => new SymbolicFloat(Size, Symbolic).Convert(size)
            };
        }

        public IFloat Divide(IFloat value)
        {
            return Create(value, (l, r) => l.Divide(r), (l, r) => l / r);
        }

        public IBool Equal(IFloat value)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return Create(value, (l, r) => l.Equal(r), (l, r) => l == r);
        }

        public IFloat Floor()
        {
            return new ConstantDouble(Math.Floor(Constant));
        }

        public IBool Greater(IFloat value)
        {
            return Create(value, (l, r) => l.Greater(r), (l, r) => l > r);
        }

        public IBool GreaterOrEqual(IFloat value)
        {
            return Create(value, (l, r) => l.GreaterOrEqual(r), (l, r) => l >= r);
        }

        public IBool Less(IFloat value)
        {
            return Create(value, (l, r) => l.Less(r), (l, r) => l < r);
        }

        public IBool LessOrEqual(IFloat value)
        {
            return Create(value, (l, r) => l.LessOrEqual(r), (l, r) => l <= r);
        }

        public IFloat Multiply(IFloat value)
        {
            return Create(value, (l, r) => l.Multiply(r), (l, r) => l * r);
        }

        public IFloat Negate()
        {
            return new ConstantDouble(-Constant);
        }

        public IBool NotEqual(IFloat value)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return Create(value, (l, r) => l.NotEqual(r), (l, r) => l != r);
        }

        public IBool Ordered(IFloat value)
        {
            return Create(value, (l, r) => l.Ordered(r), (l, r) => !(double.IsNaN(l) || double.IsNaN(r)));
        }

        public IFloat Power(IFloat value)
        {
            return Create(value, (l, r) => l.Power(r), Math.Pow);
        }

        public IFloat Remainder(IFloat value)
        {
            return Create(value, (l, r) => l.Remainder(r), Math.IEEERemainder);
        }

        public IFloat Subtract(IFloat value)
        {
            return Create(value, (l, r) => l.Subtract(r), (l, r) => l - r);
        }

        public ISigned ToSigned(Bits size)
        {
            return ConstantSigned.Create(size, (BigInteger) Constant);
        }

        public IUnsigned ToUnsigned(Bits size)
        {
            return ConstantUnsigned.Create(size, (BigInteger) Constant);
        }

        public IBool Unordered(IFloat value)
        {
            return Create(value, (l, r) => l.Unordered(r), (l, r) => double.IsNaN(l) || double.IsNaN(r));
        }

        private IFloat Create(IFloat other,
            Func<SymbolicFloat, IFloat, IFloat> symbolic,
            Func<double, double, double> constant)
        {
            return Create(other, symbolic, (l, r) => new ConstantDouble(constant(l, r)));
        }

        private IBool Create(IFloat other,
            Func<SymbolicFloat, IFloat, IBool> symbolic,
            Func<double, double, bool> constant)
        {
            return Create(other, symbolic, (l, r) => new ConstantBool(constant(l, r)));
        }

        private TValue Create<TValue>(IFloat other,
            Func<SymbolicFloat, IFloat, TValue> symbolic,
            Func<double, double, TValue> constant)
        {
            return other is IConstantDouble c
                ? constant(Constant, c.Constant)
                : symbolic(new SymbolicFloat(Size, Symbolic), other);
        }
    }
}
