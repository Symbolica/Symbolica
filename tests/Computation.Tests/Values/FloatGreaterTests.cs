﻿using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatGreaterTests
{
    private static readonly DisposableContext Context = new();

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentConstants(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatGreater.Create(constantLeft, constantRight).AsConstant(Context);
        var symbolic = FloatGreater.Create(symbolicLeft, symbolicRight).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatGreater.Create(constantLeft, constantRight).AsBitVector(Context).Simplify();
        var symbolic = FloatGreater.Create(symbolicLeft, symbolicRight).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatGreater.Create(constantLeft, constantRight).AsBool(Context).Simplify();
        var symbolic = FloatGreater.Create(symbolicLeft, symbolicRight).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}