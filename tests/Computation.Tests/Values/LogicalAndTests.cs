using System;
using System.Linq;
using FluentAssertions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class LogicalAndTests
{
    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var solver = PooledSolver.Create();

        using var bv0 = LogicalAnd.Create(left0, right0).AsBitVector(solver);
        using var result0 = bv0.Simplify();

        using var bv1 = LogicalAnd.Create(left1, right1).AsBitVector(solver);
        using var result1 = bv1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var solver = PooledSolver.Create();

        using var b0 = LogicalAnd.Create(left0, right0).AsBool(solver);
        using var result0 = b0.Simplify();

        using var b1 = LogicalAnd.Create(left1, right1).AsBool(solver);
        using var result1 = b1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Fact]
    private void ShouldBeFalseWhenAndNegation()
    {
        static IValue CreateSymbol(string name) =>
            Symbol.Create((Bits) 32, name, Enumerable.Empty<Func<IValue, IValue>>());

        var x0 = CreateSymbol("x0");
        var x1 = CreateSymbol("x1");

        var result = LogicalAnd.Create(LogicalAnd.Create(x0, x1), LogicalNot.Create(x0));

        result
            .Should()
            .BeEquivalentTo(new ConstantBool(false));
    }

    [Fact]
    private void ShouldBeFalseWhenAndNegationAfterDeMorgan()
    {
        static IValue CreateSymbol(string name) =>
            Symbol.Create((Bits) 32, name, Enumerable.Empty<Func<IValue, IValue>>());

        var x0 = CreateSymbol("x0");
        var x1 = CreateSymbol("x1");

        var result = LogicalAnd.Create(
            LogicalNot.Create(LogicalOr.Create(LogicalNot.Create(x0), LogicalNot.Create(x1))),
            LogicalNot.Create(x0));

        result
            .Should()
            .BeEquivalentTo(new ConstantBool(false));
    }

    [Fact]
    private void ShouldAnnhilitateNegationInsideOr()
    {
        static IValue CreateSymbol(string name) =>
            Symbol.Create((Bits) 32, name, Enumerable.Empty<Func<IValue, IValue>>());

        var x0 = CreateSymbol("x0");
        var x1 = CreateSymbol("x1");

        var result0 = LogicalAnd.Create(
            LogicalOr.Create(x0, LogicalNot.Create(x1)),
            x1);

        result0
            .Should()
            .BeEquivalentTo(LogicalAnd.Create(x0, x1));

        var result1 = LogicalAnd.Create(
                    LogicalOr.Create(x0, x1),
                    LogicalAnd.Create(LogicalNot.Create(x0), LogicalNot.Create(x1)));
        result1
            .Should().BeEquivalentTo(new ConstantBool(false));
    }

    [Fact]
    private void ShouldApplyAbsorption()
    {
        static IValue CreateSymbol(string name) =>
            Symbol.Create((Bits) 32, name, Enumerable.Empty<Func<IValue, IValue>>());

        var x0 = CreateSymbol("x0");
        var x1 = CreateSymbol("x1");
        var x2 = CreateSymbol("x2");

        var result1 = LogicalAnd.Create(LogicalOr.Create(x0, x1), x1);
        result1.Should().BeEquivalentTo(Logical.Create(x1));

        var result2 = LogicalAnd.Create(x1, LogicalOr.Create(x0, x1));
        result2.Should().BeEquivalentTo(Logical.Create(x1));

        LogicalAnd.Create(
            LogicalAnd.Create(x2, x1),
            LogicalOr.Create(x0, x1)).Should().BeEquivalentTo(LogicalAnd.Create(x2, x1));

        LogicalAnd.Create(
            LogicalAnd.Create(x2, LogicalOr.Create(x0, x1)),
            x1)
            .Should().BeEquivalentTo(LogicalAnd.Create(x2, x1));
    }
}
