using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicReal : IFloat
    {
        private readonly Func<Context, RealExpr> _symbolic;

        public SymbolicReal(Bits size, Func<Context, RealExpr> symbolic)
        {
            Size = size;
            _symbolic = symbolic;
        }

        public Bits Size { get; }

        public BigInteger AsConstant(Context context)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IValue GetValue(IPersistentSpace space, IBool[] constraints)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IBitwise AsBitwise()
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IBitVector AsBitVector(ICollectionFactory collectionFactory)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IUnsigned AsUnsigned()
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public ISigned AsSigned()
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IBool AsBool()
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IFloat AsFloat()
        {
            return this;
        }

        public IValue IfElse(IBool predicate, IValue falseValue)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public Func<Context, FPExpr> Symbolic => throw new UnsupportedSymbolicArithmeticException();

        public IFloat Add(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IFloat Ceiling()
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IFloat Convert(Bits size)
        {
            return new SymbolicReal(size, _symbolic);
        }

        public IFloat Divide(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IBool Equal(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IFloat Floor()
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IBool Greater(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IBool GreaterOrEqual(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IBool Less(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IBool LessOrEqual(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IFloat Multiply(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IFloat Negate()
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IBool NotEqual(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IBool Ordered(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IFloat Power(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IFloat Remainder(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IFloat Subtract(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public ISigned ToSigned(Bits size)
        {
            return new SymbolicInteger(size, c => c.MkInt2BV((uint) size, c.MkReal2Int(_symbolic(c))));
        }

        public IUnsigned ToUnsigned(Bits size)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }

        public IBool Unordered(IFloat value)
        {
            throw new UnsupportedSymbolicArithmeticException();
        }
    }
}
