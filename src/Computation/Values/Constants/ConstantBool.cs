﻿using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;

namespace Symbolica.Computation.Values.Constants
{
    internal sealed class ConstantBool : Bool, IConstantValue
    {
        private readonly bool _value;

        public ConstantBool(bool value)
        {
            _value = value;
        }

        public override BoolExpr AsBool(Context context)
        {
            return context.MkBool(_value);
        }

        public ConstantBitVector AsBitVector(ICollectionFactory collectionFactory)
        {
            return AsUnsigned().AsBitVector(collectionFactory);
        }

        public ConstantUnsigned AsUnsigned()
        {
            return ConstantUnsigned.Create(Size, _value ? BigInteger.One : BigInteger.Zero);
        }

        public ConstantSigned AsSigned()
        {
            return ConstantSigned.Create(Size, _value ? BigInteger.One : BigInteger.Zero);
        }

        public ConstantBool AsBool()
        {
            return this;
        }

        public ConstantSingle AsSingle()
        {
            return AsSigned().AsSingle();
        }

        public ConstantDouble AsDouble()
        {
            return AsSigned().AsDouble();
        }

        public static implicit operator bool(ConstantBool value)
        {
            return value._value;
        }
    }
}