using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicBool : IBool
    {
        public SymbolicBool(Func<Context, BoolExpr> symbolic)
        {
            Symbolic = symbolic;
        }

        public Bits Size => Bits.One;

        public BigInteger AsConstant(Context context)
        {
            var expr = Symbolic(context).Simplify();

            return expr.IsFalse != expr.IsTrue
                ? expr.IsTrue
                    ? BigInteger.One
                    : BigInteger.Zero
                : throw new IrreducibleSymbolicExpressionException();
        }

        public IValue GetValue(IPersistentSpace space, IBool[] constraints)
        {
            using var model = space.GetModel(constraints);

            return new ConstantBool(model.Evaluate(Symbolic).IsTrue);
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
            return new SymbolicInteger(Size, c =>
                (BitVecExpr) c.MkITE(Symbolic(c), c.MkBV(new[] {true}), c.MkBV(new[] {false})));
        }

        public ISigned AsSigned()
        {
            return AsUnsigned().AsSigned();
        }

        public IBool AsBool()
        {
            return this;
        }

        public IFloat AsFloat()
        {
            return AsUnsigned().AsFloat();
        }

        public IValue IfElse(IBool predicate, IValue falseValue)
        {
            return Create(falseValue.AsBool(), (c, t, f) => (BoolExpr) c.MkITE(predicate.Symbolic(c), t, f));
        }

        public Func<Context, BoolExpr> Symbolic { get; }

        public IProposition GetProposition(IPersistentSpace space, IBool[] constraints)
        {
            return SymbolicProposition.Create(space, this, constraints);
        }

        public IBitwise And(IBitwise value)
        {
            return Create(value.AsBool(), (c, l, r) => c.MkAnd(l, r));
        }

        public IBool Equal(IBitwise value)
        {
            return Create(value.AsBool(), (c, l, r) => c.MkEq(l, r));
        }

        public IBool Not()
        {
            return new SymbolicBool(c => c.MkNot(Symbolic(c)));
        }

        public IBool NotEqual(IBitwise value)
        {
            return Create(value.AsBool(), (c, l, r) => c.MkNot(c.MkEq(l, r)));
        }

        public IBitwise Or(IBitwise value)
        {
            return Create(value.AsBool(), (c, l, r) => c.MkOr(l, r));
        }

        public IValue Select(IValue trueValue, IValue falseValue)
        {
            return trueValue.IfElse(this, falseValue);
        }

        public IBitwise Xor(IBitwise value)
        {
            return Create(value.AsBool(), (c, l, r) => c.MkXor(l, r));
        }

        private SymbolicBool Create(IBool other, Func<Context, BoolExpr, BoolExpr, BoolExpr> symbolic)
        {
            return new(c => symbolic(c, Symbolic(c), other.Symbolic(c)));
        }
    }
}
