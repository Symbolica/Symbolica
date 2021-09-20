using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class ConstantBool : IBool, IConstantBool
    {
        public ConstantBool(bool constant)
        {
            Constant = constant;
        }

        public Bits Size => Bits.One;

        public BigInteger AsConstant(Context context)
        {
            return Constant ? BigInteger.One : BigInteger.Zero;
        }

        public IValue GetValue(IPersistentSpace space, IBool[] constraints)
        {
            return this;
        }

        public IBitwise AsBitwise()
        {
            return this;
        }

        public IBitVector AsBitVector(ICollectionFactory collectionFactory)
        {
            return AsUnsigned().AsBitVector(collectionFactory);
        }

        public IUnsigned AsUnsigned()
        {
            return ConstantUnsigned.Create(Size, Constant ? BigInteger.One : BigInteger.Zero);
        }

        public ISigned AsSigned()
        {
            return ConstantSigned.Create(Size, Constant ? BigInteger.One : BigInteger.Zero);
        }

        public IBool AsBool()
        {
            return this;
        }

        public IFloat AsFloat()
        {
            return AsSigned().AsFloat();
        }

        public IValue IfElse(IBool predicate, IValue falseValue)
        {
            return new SymbolicBool(Symbolic).IfElse(predicate, falseValue);
        }

        public Func<Context, BoolExpr> Symbolic => c => c.MkBool(Constant);

        public IProposition GetProposition(IPersistentSpace space, IBool[] constraints)
        {
            return new ConstantProposition(space, Constant);
        }

        public IBitwise And(IBitwise value)
        {
            return Create(value.AsBool(), (l, r) => l.And(r), (l, r) => l && r);
        }

        public IBool Equal(IBitwise value)
        {
            return Create(value.AsBool(), (l, r) => l.Equal(r), (l, r) => l == r);
        }

        public IBool Not()
        {
            return new ConstantBool(!Constant);
        }

        public IBool NotEqual(IBitwise value)
        {
            return Create(value.AsBool(), (l, r) => l.NotEqual(r), (l, r) => l != r);
        }

        public IBitwise Or(IBitwise value)
        {
            return Create(value.AsBool(), (l, r) => l.Or(r), (l, r) => l || r);
        }

        public IValue Select(IValue trueValue, IValue falseValue)
        {
            return Constant ? trueValue : falseValue;
        }

        public IBitwise Xor(IBitwise value)
        {
            return Create(value.AsBool(), (l, r) => l.Xor(r), (l, r) => l ^ r);
        }

        public bool Constant { get; }

        private IBool Create(IBool other,
            Func<SymbolicBool, IBool, IBitwise> symbolic,
            Func<bool, bool, bool> constant)
        {
            return other is IConstantBool c
                ? new ConstantBool(constant(Constant, c.Constant))
                : symbolic(new SymbolicBool(Symbolic), other).AsBool();
        }
    }
}
