using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    [Serializable]
    internal sealed class SymbolicBool : IBool
    {
        public SymbolicBool(IFunc<Context, BoolExpr> symbolic)
        {
            Symbolic = symbolic;
        }

        public Bits Size => Bits.One;

        public BigInteger AsConstant(IContextFactory contextFactory)
        {
            using var handle = contextFactory.Create();
            var expr = Symbolic.Run(handle.Context).Simplify();

            return expr.IsFalse != expr.IsTrue
                ? expr.IsTrue
                    ? BigInteger.One
                    : BigInteger.Zero
                : throw new IrreducibleSymbolicExpressionException();
        }

        public IValue GetValue(IPersistentSpace space, IBool[] constraints)
        {
            using var model = space.GetModel(constraints);

            return new ConstantBool(model.Evaluate(Symbolic.Run).IsTrue);
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
            return new SymbolicInteger(
                Size,
                new ContextFuncs.MkITE<BitVecExpr>(
                    Symbolic,
                    new ContextFuncs.MkBVOfBits(new[] { true }),
                    new ContextFuncs.MkBVOfBits(new[] { false })));
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
            return new SymbolicBool(
                new ContextFuncs.MkITE<BoolExpr>(predicate.Symbolic, Symbolic, falseValue.AsBool().Symbolic));
        }

        public IFunc<Context, BoolExpr> Symbolic { get; }

        public IProposition GetProposition(IPersistentSpace space, IBool[] constraints)
        {
            return SymbolicProposition.Create(space, this, constraints);
        }

        public IBitwise And(IBitwise value)
        {
            return new SymbolicBool(new ContextFuncs.MkAnd(Symbolic, value.AsBool().Symbolic));
        }

        public IBool Equal(IBitwise value)
        {
            return new SymbolicBool(new ContextFuncs.MkEq(Symbolic, value.AsBool().Symbolic));
        }

        public IBool Not()
        {
            return new SymbolicBool(new ContextFuncs.MkNot(Symbolic));
        }

        public IBool NotEqual(IBitwise value)
        {
            return new SymbolicBool(new ContextFuncs.MkNot(new ContextFuncs.MkEq(Symbolic, value.AsBool().Symbolic)));
        }

        public IBitwise Or(IBitwise value)
        {
            return new SymbolicBool(new ContextFuncs.MkOr(Symbolic, value.AsBool().Symbolic));
        }

        public IValue Select(IValue trueValue, IValue falseValue)
        {
            return trueValue.IfElse(this, falseValue);
        }

        public IBitwise Xor(IBitwise value)
        {
            return new SymbolicBool(new ContextFuncs.MkXor(Symbolic, value.AsBool().Symbolic));
        }

        private SymbolicBool Create(IBool other, IFunc<Context, IFunc<BoolExpr, IFunc<BoolExpr, BoolExpr>>> symbolic)
        {
            return new(symbolic.Apply(Symbolic).Apply(other.Symbolic));
        }
    }
}
