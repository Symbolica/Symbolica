using System.Numerics;
using FluentAssertions;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class AndTests
{
    private static readonly Context<ContextHandle> Context = new();

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentConstants(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = And.Create(constantLeft, constantRight).AsConstant(Context);
        var symbolic = And.Create(symbolicLeft, symbolicRight).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = And.Create(constantLeft, constantRight).AsBitVector(Context).Simplify();
        var symbolic = And.Create(symbolicLeft, symbolicRight).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = And.Create(constantLeft, constantRight).AsBool(Context).Simplify();
        var symbolic = And.Create(symbolicLeft, symbolicRight).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(UnaryTestData))]
    private void ShouldShortCircuitToZeroWhenEitherArgumentIsZero(
        ConstantUnsigned constant, SymbolicUnsigned symbolic)
    {
        static Expr And(IValue left, IValue right)
        {
            return Values.And.Create(left, right).AsBitVector(Context).Simplify();
        }

        var zero = ConstantUnsigned.Create(constant.Size, BigInteger.Zero);
        new[] {
            And(constant, zero),
            And(zero, constant),
            And(symbolic, zero),
            And(zero, symbolic)
        }.Should().AllBeEquivalentTo(zero.AsBitVector(Context).Simplify());
    }

    [Theory]
    [ClassData(typeof(UnaryTestData))]
    private void ShouldShortCircuitToOtherWhenEitherArgumentIsOne(
        ConstantUnsigned constant, SymbolicUnsigned symbolic)
    {
        static Expr And(IValue left, IValue right)
        {
            return Values.And.Create(left, right).AsBitVector(Context).Simplify();
        }

        var ones = Not.Create(ConstantUnsigned.Create(constant.Size, BigInteger.Zero));
        new[] {
            And(constant, ones),
            And(ones, constant),
            And(symbolic, ones),
            And(ones, symbolic)
        }.Should().AllBeEquivalentTo(constant.AsBitVector(Context).Simplify());
    }
}
