﻿using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class UnsignedLessTests
{
    private static readonly IContext Context = DisposableContext.Create();

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentConstants(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = UnsignedLess.Create(constantLeft, constantRight).AsConstant(Context);
        var symbolic = UnsignedLess.Create(symbolicLeft, symbolicRight).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = UnsignedLess.Create(constantLeft, constantRight).AsBitVector(Context).Simplify();
        var symbolic = UnsignedLess.Create(symbolicLeft, symbolicRight).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = UnsignedLess.Create(constantLeft, constantRight).AsBool(Context).Simplify();
        var symbolic = UnsignedLess.Create(symbolicLeft, symbolicRight).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}
