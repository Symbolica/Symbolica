using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Constants;

internal sealed record ConstantDouble : Float, IConstantValue
{
    private readonly double _value;

    public ConstantDouble(double value)
        : base((Bits) 64U)
    {
        _value = value;
    }

    public override FPExpr AsFloat(ISolver solver)
    {
        using var sort = Size.GetSort(solver);
        return solver.Context.MkFP(_value, sort);
    }

    public ConstantBitVector AsBitVector(ICollectionFactory collectionFactory)
    {
        return AsUnsigned().AsBitVector(collectionFactory);
    }

    public ConstantUnsigned AsUnsigned()
    {
        return AsSigned().AsUnsigned();
    }

    public ConstantSigned AsSigned()
    {
        return ConstantSigned.Create(Size, BitConverter.DoubleToInt64Bits(_value));
    }

    public ConstantBool AsBool()
    {
        return AsUnsigned().AsBool();
    }

    public ConstantSingle AsSingle()
    {
        return ConstantSingle.Create(AsSigned());
    }

    public ConstantDouble AsDouble()
    {
        return this;
    }

    public static implicit operator double(ConstantDouble value)
    {
        return value._value;
    }

    public static ConstantDouble Create(ConstantSigned value)
    {
        return value.Size == (Bits) 64U
            ? new ConstantDouble(BitConverter.Int64BitsToDouble((long) (BigInteger) value))
            : throw new InconsistentExpressionSizesException(value.Size, (Bits) 64U);
    }
}
