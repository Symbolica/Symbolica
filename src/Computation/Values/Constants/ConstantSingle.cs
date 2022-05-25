using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Constants;

internal sealed record ConstantSingle : Float, IConstantValue
{
    private readonly float _value;

    public ConstantSingle(float value)
        : base((Bits) 32U)
    {
        _value = value;
    }

    public override ISet<IValue> Symbols => new HashSet<IValue>();

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

    public override bool Equals(IValue? other)
    {
        return Equals(other as IConstantValue);
    }

    public bool Equals(IConstantValue? other)
    {
        return AsUnsigned().Equals(other);
    }

    public bool Equals(ConstantSingle? other)
    {
        return Equals(other as IConstantValue);
    }

    public override int GetHashCode()
    {
        return AsUnsigned().GetHashCode();
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

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return (new(), Equals(other));
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : this;
    }

    public override object ToJson()
    {
        return ((BigInteger) AsUnsigned()).ToString();
    }

    public override int GetEquivalencyHash(bool includeSubs)
    {
        return ((BigInteger) AsUnsigned()).GetHashCode();
    }
}
