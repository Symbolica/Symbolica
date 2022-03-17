using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class Expression : IExpression
{
    private readonly ICollectionFactory _collectionFactory;
    private readonly IValue _value;

    public Expression(ICollectionFactory collectionFactory,
        IValue value)
    {
        _collectionFactory = collectionFactory;
        _value = value;
    }

    public Bits Size => _value.Size;

    public BigInteger GetSingleValue(ISpace space)
    {
        return _value is IConstantValue v
            ? v.AsUnsigned()
            : GetSingleValue((IPersistentSpace) space);
    }

    public BigInteger GetExampleValue(ISpace space)
    {
        return _value is IConstantValue v
            ? v.AsUnsigned()
            : GetExampleValue((IPersistentSpace) space);
    }

    public IProposition GetProposition(ISpace space)
    {
        return _value is IConstantValue v
            ? ConstantProposition.Create(space, v)
            : SymbolicProposition.Create((IPersistentSpace) space, _value);
    }

    public IExpression Add(IExpression expression)
    {
        return Create(expression, Values.Add.Create);
    }

    public IExpression And(IExpression expression)
    {
        return Create(expression, (l, r) => l is Bool || r is Bool
            ? LogicalAnd.Create(l, r)
            : Values.And.Create(l, r));
    }

    public IExpression ArithmeticShiftRight(IExpression expression)
    {
        return Create(expression, Values.ArithmeticShiftRight.Create);
    }

    public IExpression Equal(IExpression expression)
    {
        return Create(expression, Values.Equal.Create);
    }

    public IExpression FloatAdd(IExpression expression)
    {
        return Create(expression, Values.FloatAdd.Create);
    }

    public IExpression FloatCeiling()
    {
        return Create(Values.FloatCeiling.Create);
    }

    public IExpression FloatConvert(Bits size)
    {
        return Create(v => Values.FloatConvert.Create(size, v));
    }

    public IExpression FloatDivide(IExpression expression)
    {
        return Create(expression, Values.FloatDivide.Create);
    }

    public IExpression FloatEqual(IExpression expression)
    {
        return Create(expression, Values.FloatEqual.Create);
    }

    public IExpression FloatFloor()
    {
        return Create(Values.FloatFloor.Create);
    }

    public IExpression FloatGreater(IExpression expression)
    {
        return Create(expression, Values.FloatGreater.Create);
    }

    public IExpression FloatGreaterOrEqual(IExpression expression)
    {
        return Create(expression, Values.FloatGreaterOrEqual.Create);
    }

    public IExpression FloatLess(IExpression expression)
    {
        return Create(expression, Values.FloatLess.Create);
    }

    public IExpression FloatLessOrEqual(IExpression expression)
    {
        return Create(expression, Values.FloatLessOrEqual.Create);
    }

    public IExpression FloatMultiply(IExpression expression)
    {
        return Create(expression, Values.FloatMultiply.Create);
    }

    public IExpression FloatNegate()
    {
        return Create(Values.FloatNegate.Create);
    }

    public IExpression FloatNotEqual(IExpression expression)
    {
        return Create(expression, Values.FloatNotEqual.Create);
    }

    public IExpression FloatOrdered(IExpression expression)
    {
        return Create(expression, Values.FloatOrdered.Create);
    }

    public IExpression FloatPower(IExpression expression)
    {
        return Create(expression, Values.FloatPower.Create);
    }

    public IExpression FloatRemainder(IExpression expression)
    {
        return Create(expression, Values.FloatRemainder.Create);
    }

    public IExpression FloatSubtract(IExpression expression)
    {
        return Create(expression, Values.FloatSubtract.Create);
    }

    public IExpression FloatToSigned(Bits size)
    {
        return Create(v => Values.FloatToSigned.Create(size, v));
    }

    public IExpression FloatToUnsigned(Bits size)
    {
        return Create(v => Values.FloatToUnsigned.Create(size, v));
    }

    public IExpression FloatUnordered(IExpression expression)
    {
        return Create(expression, Values.FloatUnordered.Create);
    }

    public IExpression LogicalShiftRight(IExpression expression)
    {
        return Create(expression, Values.LogicalShiftRight.Create);
    }

    public IExpression Multiply(IExpression expression)
    {
        return Create(expression, Values.Multiply.Create);
    }

    public IExpression NotEqual(IExpression expression)
    {
        return Create(expression, Values.NotEqual.Create);
    }

    public IExpression Or(IExpression expression)
    {
        return Create(expression, (l, r) => l is Bool || r is Bool
            ? LogicalOr.Create(l, r)
            : Values.Or.Create(l, r));
    }

    public IExpression Read(IExpression offset, Bits size)
    {
        return Create(offset, (b, o) => Values.Read.Create(_collectionFactory, b, o, size));
    }

    public IExpression Select(IExpression trueValue, IExpression falseValue)
    {
        return trueValue.Size == falseValue.Size
            ? Create(trueValue, falseValue, Values.Select.Create)
            : throw new InconsistentExpressionSizesException(trueValue.Size, falseValue.Size);
    }

    public IExpression ShiftLeft(IExpression expression)
    {
        return Create(expression, Values.ShiftLeft.Create);
    }

    public IExpression SignedDivide(IExpression expression)
    {
        return Create(expression, Values.SignedDivide.Create);
    }

    public IExpression SignedGreater(IExpression expression)
    {
        return Create(expression, Values.SignedGreater.Create);
    }

    public IExpression SignedGreaterOrEqual(IExpression expression)
    {
        return Create(expression, Values.SignedGreaterOrEqual.Create);
    }

    public IExpression SignedLess(IExpression expression)
    {
        return Create(expression, Values.SignedLess.Create);
    }

    public IExpression SignedLessOrEqual(IExpression expression)
    {
        return Create(expression, Values.SignedLessOrEqual.Create);
    }

    public IExpression SignedRemainder(IExpression expression)
    {
        return Create(expression, Values.SignedRemainder.Create);
    }

    public IExpression SignedToFloat(Bits size)
    {
        return Create(v => Values.SignedToFloat.Create(size, v));
    }

    public IExpression SignExtend(Bits size)
    {
        return Create(v => Values.SignExtend.Create(size, v));
    }

    public IExpression Subtract(IExpression expression)
    {
        return Create(expression, Values.Subtract.Create);
    }

    public IExpression Truncate(Bits size)
    {
        return Create(v => Values.Truncate.Create(size, v));
    }

    public IExpression UnsignedDivide(IExpression expression)
    {
        return Create(expression, Values.UnsignedDivide.Create);
    }

    public IExpression UnsignedGreater(IExpression expression)
    {
        return Create(expression, Values.UnsignedGreater.Create);
    }

    public IExpression UnsignedGreaterOrEqual(IExpression expression)
    {
        return Create(expression, Values.UnsignedGreaterOrEqual.Create);
    }

    public IExpression UnsignedLess(IExpression expression)
    {
        return Create(expression, Values.UnsignedLess.Create);
    }

    public IExpression UnsignedLessOrEqual(IExpression expression)
    {
        return Create(expression, Values.UnsignedLessOrEqual.Create);
    }

    public IExpression UnsignedRemainder(IExpression expression)
    {
        return Create(expression, Values.UnsignedRemainder.Create);
    }

    public IExpression UnsignedToFloat(Bits size)
    {
        return Create(v => Values.UnsignedToFloat.Create(size, v));
    }

    public IExpression Write(IExpression offset, IExpression value)
    {
        return Size == offset.Size
            ? Create(offset, value, (b, o, v) => Values.Write.Create(_collectionFactory, b, o, v))
            : throw new InconsistentExpressionSizesException(Size, offset.Size);
    }

    public IExpression Xor(IExpression expression)
    {
        return Create(expression, (l, r) => l is Bool || r is Bool
            ? LogicalXor.Create(l, r)
            : Values.Xor.Create(l, r));
    }

    public IExpression ZeroExtend(Bits size)
    {
        return Create(v => Values.ZeroExtend.Create(size, v));
    }

    private IExpression Create(Func<IValue, IValue> func)
    {
        return new Expression(_collectionFactory,
            func(_value));
    }

    private IExpression Create(IExpression y, Func<IValue, IValue, IValue> func)
    {
        return Size == y.Size
            ? new Expression(_collectionFactory,
                func(_value, ((Expression) y)._value))
            : throw new InconsistentExpressionSizesException(Size, y.Size);
    }

    private IExpression Create(IExpression y, IExpression z, Func<IValue, IValue, IValue, IValue> func)
    {
        return new Expression(_collectionFactory,
            func(_value, ((Expression) y)._value, ((Expression) z)._value));
    }

    private BigInteger GetSingleValue(IPersistentSpace space)
    {
        using var context = space.CreateContext();

        return _value is Float && _value.AsFloat(context).Simplify().IsFPNaN
            ? _value.Size.GetNan(context)
            : context.GetSingleValue(_value.AsBitVector(context));
    }

    private BigInteger GetExampleValue(IPersistentSpace space)
    {
        using var context = space.CreateContext();

        return context.GetExampleValue(_value.AsBitVector(context));
    }

    public static IExpression CreateSymbolic(ICollectionFactory collectionFactory,
        Bits size, string name, IEnumerable<Func<IExpression, IExpression>> assertions)
    {
        return new Expression(collectionFactory,
            Symbol.Create(size, name, assertions.Select(a => new Func<IValue, IValue>(
                v => ((Expression) a(new Expression(collectionFactory, v)))._value))));
    }
}
