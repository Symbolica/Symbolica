using System.Numerics;
using FluentAssertions;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class OrTests
{
    private static readonly Context<ContextHandle> Context = new();

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentConstants(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = Or.Create(constantLeft, constantRight).AsConstant(Context);
        var symbolic = Or.Create(symbolicLeft, symbolicRight).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = Or.Create(constantLeft, constantRight).AsBitVector(Context).Simplify();
        var symbolic = Or.Create(symbolicLeft, symbolicRight).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = Or.Create(constantLeft, constantRight).AsBool(Context).Simplify();
        var symbolic = Or.Create(symbolicLeft, symbolicRight).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(UnaryTestData))]
    private void ShouldShortCircuitToOtherWhenEitherArgumentIsZero(
        ConstantUnsigned constant, SymbolicUnsigned symbolic)
    {
        static Expr Or(IValue left, IValue right)
        {
            return Values.Or.Create(left, right).AsBitVector(Context).Simplify();
        }

        var zero = ConstantUnsigned.Create(constant.Size, BigInteger.Zero);
        new[] {
            Or(constant, zero),
            Or(zero, constant),
            Or(symbolic, zero),
            Or(zero, symbolic)
        }.Should().AllBeEquivalentTo(constant.AsBitVector(Context).Simplify());
    }

    [Theory]
    [ClassData(typeof(UnaryTestData))]
    private void ShouldShortCircuitToOnesWhenEitherArgumentIsOne(
        ConstantUnsigned constant, SymbolicUnsigned symbolic)
    {
        static Expr Or(IValue left, IValue right)
        {
            return Values.Or.Create(left, right).AsBitVector(Context).Simplify();
        }

        var ones = Not.Create(ConstantUnsigned.Create(constant.Size, BigInteger.Zero));
        new[] {
            Or(constant, ones),
            Or(ones, constant),
            Or(symbolic, ones),
            Or(ones, symbolic)
        }.Should().AllBeEquivalentTo(ones.AsBitVector(Context).Simplify());
    }
}
