using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Constants;

internal sealed class ConstantSingle : Float, IConstantValue
{
    private readonly float _value;

    public ConstantSingle(float value)
        : base((Bits) 32U)
    {
        _value = value;
    }

    public override FPExpr AsFloat(IContext context)
    {
        return context.CreateExpr(c => c.MkFP(_value, Size.GetSort(context)));
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
        return ConstantSigned.Create(Size, BitConverter.SingleToInt32Bits(_value));
    }

    public ConstantBool AsBool()
    {
        return AsUnsigned().AsBool();
    }

    public ConstantSingle AsSingle()
    {
        return this;
    }

    public ConstantDouble AsDouble()
    {
        return ConstantDouble.Create(AsSigned());
    }

    public static implicit operator float(ConstantSingle value)
    {
        return value._value;
    }

    public static ConstantSingle Create(ConstantSigned value)
    {
        return value.Size == (Bits) 32U
            ? new ConstantSingle(BitConverter.Int32BitsToSingle((int) (BigInteger) value))
            : throw new InconsistentExpressionSizesException(value.Size, (Bits) 32U);
    }
}
