using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class SymbolicBool : IValue
    {
        private readonly Func<Context, BoolExpr> _func;

        public SymbolicBool(Func<Context, BoolExpr> func)
        {
            _func = func;
        }

        public Bits Size => Bits.One;

        public BigInteger GetInteger(Context context)
        {
            var expr = _func(context).Simplify();

            return expr.IsFalse != expr.IsTrue
                ? expr.IsTrue
                    ? BigInteger.One
                    : BigInteger.Zero
                : throw new Exception("The boolean cannot be simplified to a constant.");
        }

        public IValue GetValue(IPersistentSpace space, SymbolicBool[] constraints)
        {
            using var model = space.GetModel(constraints);

            return new ConstantBool(model.Evaluate(_func).IsTrue);
        }

        public IProposition GetProposition(IPersistentSpace space, SymbolicBool[] constraints)
        {
            return SymbolicProposition.Create(space, this, constraints);
        }

        public IValue Add(IValue value)
        {
            return ToSymbolicBitVector().Add(value);
        }

        public IValue And(IValue value)
        {
            return Create(value, (c, l, r) => c.MkAnd(l, r));
        }

        public IValue ArithmeticShiftRight(IValue value)
        {
            return ToSymbolicBitVector().ArithmeticShiftRight(value);
        }

        public IValue Equal(IValue value)
        {
            return Create(value, (c, l, r) => c.MkEq(l, r));
        }

        public IValue FloatAdd(IValue value)
        {
            return ToSymbolicFloat().FloatAdd(value);
        }

        public IValue FloatCeiling()
        {
            return ToSymbolicFloat().FloatCeiling();
        }

        public IValue FloatConvert(Bits size)
        {
            return ToSymbolicFloat().FloatConvert(size);
        }

        public IValue FloatDivide(IValue value)
        {
            return ToSymbolicFloat().FloatDivide(value);
        }

        public IValue FloatEqual(IValue value)
        {
            return ToSymbolicFloat().FloatEqual(value);
        }

        public IValue FloatFloor()
        {
            return ToSymbolicFloat().FloatFloor();
        }

        public IValue FloatGreater(IValue value)
        {
            return ToSymbolicFloat().FloatGreater(value);
        }

        public IValue FloatGreaterOrEqual(IValue value)
        {
            return ToSymbolicFloat().FloatGreaterOrEqual(value);
        }

        public IValue FloatLess(IValue value)
        {
            return ToSymbolicFloat().FloatLess(value);
        }

        public IValue FloatLessOrEqual(IValue value)
        {
            return ToSymbolicFloat().FloatLessOrEqual(value);
        }

        public IValue FloatMultiply(IValue value)
        {
            return ToSymbolicFloat().FloatMultiply(value);
        }

        public IValue FloatNegate()
        {
            return ToSymbolicFloat().FloatNegate();
        }

        public IValue FloatNotEqual(IValue value)
        {
            return ToSymbolicFloat().FloatNotEqual(value);
        }

        public IValue FloatOrdered(IValue value)
        {
            return ToSymbolicFloat().FloatOrdered(value);
        }

        public IValue FloatRemainder(IValue value)
        {
            return ToSymbolicFloat().FloatRemainder(value);
        }

        public IValue FloatSubtract(IValue value)
        {
            return ToSymbolicFloat().FloatSubtract(value);
        }

        public IValue FloatToSigned(Bits size)
        {
            return ToSymbolicFloat().FloatToSigned(size);
        }

        public IValue FloatToUnsigned(Bits size)
        {
            return ToSymbolicFloat().FloatToUnsigned(size);
        }

        public IValue FloatUnordered(IValue value)
        {
            return ToSymbolicFloat().FloatUnordered(value);
        }

        public IValue LogicalShiftRight(IValue value)
        {
            return ToSymbolicBitVector().LogicalShiftRight(value);
        }

        public IValue Multiply(IValue value)
        {
            return ToSymbolicBitVector().Multiply(value);
        }

        IValue IValue.Not()
        {
            return Not();
        }

        public IValue NotEqual(IValue value)
        {
            return Create(value, (c, l, r) => c.MkNot(c.MkEq(l, r)));
        }

        public IValue Or(IValue value)
        {
            return Create(value, (c, l, r) => c.MkOr(l, r));
        }

        public IValue Read(ICollectionFactory collectionFactory, IValue offset, Bits size)
        {
            return ToSymbolicBitVector().Read(collectionFactory, offset, size);
        }

        public IValue Select(IValue trueValue, IValue falseValue)
        {
            return trueValue.Select(_func, falseValue);
        }

        public IValue Select(Func<Context, BoolExpr> predicate, IValue falseValue)
        {
            return Create(falseValue, (c, t, f) => (BoolExpr) c.MkITE(predicate(c), t, f));
        }

        public IValue ShiftLeft(IValue value)
        {
            return ToSymbolicBitVector().ShiftLeft(value);
        }

        public IValue SignedDivide(IValue value)
        {
            return ToSymbolicBitVector().SignedDivide(value);
        }

        public IValue SignedGreater(IValue value)
        {
            return ToSymbolicBitVector().SignedGreater(value);
        }

        public IValue SignedGreaterOrEqual(IValue value)
        {
            return ToSymbolicBitVector().SignedGreaterOrEqual(value);
        }

        public IValue SignedLess(IValue value)
        {
            return ToSymbolicBitVector().SignedLess(value);
        }

        public IValue SignedLessOrEqual(IValue value)
        {
            return ToSymbolicBitVector().SignedLessOrEqual(value);
        }

        public IValue SignedRemainder(IValue value)
        {
            return ToSymbolicBitVector().SignedRemainder(value);
        }

        public IValue SignedToFloat(Bits size)
        {
            return ToSymbolicBitVector().SignedToFloat(size);
        }

        public IValue SignExtend(Bits size)
        {
            return ToSymbolicBitVector().SignExtend(size);
        }

        public IValue Subtract(IValue value)
        {
            return ToSymbolicBitVector().Subtract(value);
        }

        public IValue Truncate(Bits size)
        {
            return ToSymbolicBitVector().Truncate(size);
        }

        public IValue UnsignedDivide(IValue value)
        {
            return ToSymbolicBitVector().UnsignedDivide(value);
        }

        public IValue UnsignedGreater(IValue value)
        {
            return ToSymbolicBitVector().UnsignedGreater(value);
        }

        public IValue UnsignedGreaterOrEqual(IValue value)
        {
            return ToSymbolicBitVector().UnsignedGreaterOrEqual(value);
        }

        public IValue UnsignedLess(IValue value)
        {
            return ToSymbolicBitVector().UnsignedLess(value);
        }

        public IValue UnsignedLessOrEqual(IValue value)
        {
            return ToSymbolicBitVector().UnsignedLessOrEqual(value);
        }

        public IValue UnsignedRemainder(IValue value)
        {
            return ToSymbolicBitVector().UnsignedRemainder(value);
        }

        public IValue UnsignedToFloat(Bits size)
        {
            return ToSymbolicBitVector().UnsignedToFloat(size);
        }

        public IValue Write(ICollectionFactory collectionFactory, IValue offset, IValue value)
        {
            return ToSymbolicBitVector().Write(collectionFactory, offset, value);
        }

        public IValue Xor(IValue value)
        {
            return Create(value, (c, l, r) => c.MkXor(l, r));
        }

        public IValue ZeroExtend(Bits size)
        {
            return ToSymbolicBitVector().ZeroExtend(size);
        }

        public SymbolicBitVector ToSymbolicBitVector()
        {
            return new(Size, c => (BitVecExpr) c.MkITE(_func(c), c.MkBV(new[] {true}), c.MkBV(new[] {false})));
        }

        public SymbolicBool ToSymbolicBool()
        {
            return this;
        }

        public SymbolicFloat ToSymbolicFloat()
        {
            return ToSymbolicBitVector().ToSymbolicFloat();
        }

        public SymbolicBool Not()
        {
            return new(c => c.MkNot(_func(c)));
        }

        private IValue Create(IValue other, Func<Context, BoolExpr, BoolExpr, BoolExpr> func)
        {
            return new SymbolicBool(c => func(c, _func(c), other.ToSymbolicBool()._func(c)));
        }

        public bool IsSatisfiable(IModel model)
        {
            return model.IsSatisfiable(_func);
        }

        public static IModel GetModel(IModelFactory modelFactory, IEnumerable<SymbolicBool> assertions)
        {
            return modelFactory.Create(assertions.Select(a => a._func));
        }
    }
}
