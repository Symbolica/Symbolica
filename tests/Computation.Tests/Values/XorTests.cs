using System.Numerics;
using FluentAssertions;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class XorTests
{
    private static readonly Context<ContextHandle> Context = new();

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentConstants(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = Xor.Create(constantLeft, constantRight).AsConstant(Context);
        var symbolic = Xor.Create(symbolicLeft, symbolicRight).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = Xor.Create(constantLeft, constantRight).AsBitVector(Context).Simplify();
        var symbolic = Xor.Create(symbolicLeft, symbolicRight).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicUnsigned symbolicLeft, SymbolicUnsigned symbolicRight)
    {
        var constant = Xor.Create(constantLeft, constantRight).AsBool(Context).Simplify();
        var symbolic = Xor.Create(symbolicLeft, symbolicRight).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(UnaryTestData))]
    private void ShouldShortCircuitToOtherWhenEitherArgumentIsZero(
        ConstantUnsigned constant, SymbolicUnsigned symbolic)
    {
        static Expr Xor(IValue left, IValue right)
        {
            return Values.Xor.Create(left, right).AsBitVector(Context).Simplify();
        }

        var zero = ConstantUnsigned.Create(constant.Size, BigInteger.Zero);
        new[] {
            Xor(constant, zero),
            Xor(zero, constant),
            Xor(symbolic, zero),
            Xor(zero, symbolic)
        }.Should().AllBeEquivalentTo(constant.AsBitVector(Context).Simplify());
    }

    [Theory]
    [ClassData(typeof(UnaryTestData))]
    private void ShouldShortCircuitToNotOtherWhenEitherArgumentIsOne(
        ConstantUnsigned constant, SymbolicUnsigned symbolic)
    {
        static Expr Xor(IValue left, IValue right)
        {
            return Values.Xor.Create(left, right).AsBitVector(Context).Simplify();
        }

        var ones = Not.Create(ConstantUnsigned.Create(constant.Size, BigInteger.Zero));
        new[] {
            Xor(constant, ones),
            Xor(ones, constant),
            Xor(symbolic, ones),
            Xor(ones, symbolic)
        }.Should().AllBeEquivalentTo(Not.Create(constant).AsBitVector(Context).Simplify());
    }
}
