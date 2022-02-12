﻿using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatToUnsignedTests
{
    private static readonly DisposableContext Context = new();

    [Theory]
    [ClassData(typeof(SingleToUnsignedTestData))]
    [ClassData(typeof(DoubleToUnsignedTestData))]
    private void ShouldCreateEquivalentConstants(Bits size,
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatToUnsigned.Create(size, constantValue).AsConstant(Context);
        var symbolic = FloatToUnsigned.Create(size, symbolicValue).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleToUnsignedTestData))]
    [ClassData(typeof(DoubleToUnsignedTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatToUnsigned.Create(size, constantValue).AsBitVector(Context).Simplify();
        var symbolic = FloatToUnsigned.Create(size, symbolicValue).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleToUnsignedTestData))]
    [ClassData(typeof(DoubleToUnsignedTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatToUnsigned.Create(size, constantValue).AsBool(Context).Simplify();
        var symbolic = FloatToUnsigned.Create(size, symbolicValue).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleToUnsignedTestData))]
    [ClassData(typeof(DoubleToUnsignedTestData))]
    private void ShouldCreateEquivalentFloats(Bits size,
        IConstantValue constantValue,
        SymbolicFloat symbolicValue)
    {
        var constant = FloatToUnsigned.Create(size, constantValue).AsFloat(Context).Simplify();
        var symbolic = FloatToUnsigned.Create(size, symbolicValue).AsFloat(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}