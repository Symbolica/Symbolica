using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class ConstantSingle : IFloat, IConstantSingle
    {
        public ConstantSingle(float constant)
        {
            Constant = constant;
        }

        public float Constant { get; }
        public Bits Size => (Bits)32U;

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
            return ConstantSigned.Create(Size, BitConverter.SingleToInt32Bits(Constant));
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

        public IFunc<Context, FPExpr> Symbolic => new ContextFuncs.MkFP(Constant, new ContextFuncs.GetSort(Size));

        public IFloat Add(IFloat value)
        {
            return Create(value, (l, r) => l.Add(r), (l, r) => l + r);
        }

        public IFloat Ceiling()
        {
            return new ConstantSingle(MathF.Ceiling(Constant));
        }

        public IFloat Convert(Bits size)
        {
            return (uint)size switch
            {
                32U => this,
                64U => new ConstantDouble(Constant),
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
            return new ConstantSingle(MathF.Floor(Constant));
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
            return new ConstantSingle(-Constant);
        }

        public IBool NotEqual(IFloat value)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return Create(value, (l, r) => l.NotEqual(r), (l, r) => l != r);
        }

        public IBool Ordered(IFloat value)
        {
            return Create(value, (l, r) => l.Ordered(r), (l, r) => !(float.IsNaN(l) || float.IsNaN(r)));
        }

        public IFloat Power(IFloat value)
        {
            return Create(value, (l, r) => l.Power(r), MathF.Pow);
        }

        public IFloat Remainder(IFloat value)
        {
            return Create(value, (l, r) => l.Remainder(r), MathF.IEEERemainder);
        }

        public IFloat Subtract(IFloat value)
        {
            return Create(value, (l, r) => l.Subtract(r), (l, r) => l - r);
        }

        public ISigned ToSigned(Bits size)
        {
            return ConstantSigned.Create(size, (BigInteger)Constant);
        }

        public IUnsigned ToUnsigned(Bits size)
        {
            return ConstantUnsigned.Create(size, (BigInteger)Constant);
        }

        public IBool Unordered(IFloat value)
        {
            return Create(value, (l, r) => l.Unordered(r), (l, r) => float.IsNaN(l) || float.IsNaN(r));
        }

        private IFloat Create(IFloat other,
            Func<SymbolicFloat, IFloat, IFloat> symbolic,
            Func<float, float, float> constant)
        {
            return Create(other, symbolic, (l, r) => new ConstantSingle(constant(l, r)));
        }

        private IBool Create(IFloat other,
            Func<SymbolicFloat, IFloat, IBool> symbolic,
            Func<float, float, bool> constant)
        {
            return Create(other, symbolic, (l, r) => new ConstantBool(constant(l, r)));
        }

        private TValue Create<TValue>(IFloat other,
            Func<SymbolicFloat, IFloat, TValue> symbolic,
            Func<float, float, TValue> constant)
        {
            return other is IConstantSingle c
                ? constant(Constant, c.Constant)
                : symbolic(new SymbolicFloat(Size, Symbolic), other);
        }
    }
}
