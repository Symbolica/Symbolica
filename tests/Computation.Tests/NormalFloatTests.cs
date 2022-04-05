using System;
using FluentAssertions;
using Microsoft.Z3;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation;

public class NormalFloatTests
{
    [Theory]
    [InlineData("0")]
    [InlineData("0.0")]
    [InlineData("-0")]
    [InlineData("-0.0")]
    [InlineData("00.00E1")]
    [InlineData("-000.000E10")]
    [InlineData("01")]
    [InlineData("-01.00E-01")]
    [InlineData("2.0")]
    [InlineData("987.654E32")]
    [InlineData("98765.4E-21")]
    [InlineData("123.421")]
    [InlineData("1.23421e+2")]
    [InlineData("1.25")]
    [InlineData("1.3")]
    [InlineData("4.5e+15")]
    [InlineData("3.14159")]
    [InlineData("1.2345678901234567e22")]
    [InlineData("3.08984926168550152811e-32")]
    [InlineData("0.00012207031")]
    [InlineData("1.0737418E+09")]
    [InlineData("1.175494351E-38")]
    [InlineData("3.402823466E+38")]
    private void ShouldCreateEquivalentRepresentationForSinglePrecision(string value)
    {
        using var solver = PooledSolver.Create();

        var normalFloat = new NormalFloat(Size.FromBits(32U), value);

        using var expr = normalFloat.AsFloat(solver);
        using var bitVector = solver.Context.MkFPToIEEEBV(expr);
        using var simplified = (BitVecNum) bitVector.Simplify();

        var actual = BitConverter.GetBytes(simplified.UInt);
        var expected = BitConverter.GetBytes(float.Parse(value));

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("0.0")]
    [InlineData("-0")]
    [InlineData("-0.0")]
    [InlineData("00.00E1")]
    [InlineData("-000.000E10")]
    [InlineData("01")]
    [InlineData("-01.00E-01")]
    [InlineData("2.0")]
    [InlineData("987.654E32")]
    [InlineData("98765.4E-21")]
    [InlineData("123.421")]
    [InlineData("1.23421e+2")]
    [InlineData("1.25")]
    [InlineData("1.3")]
    [InlineData("4.5e+15")]
    [InlineData("3.14159")]
    [InlineData("1.2345678901234567e22")]
    [InlineData("3.08984926168550152811e-32")]
    [InlineData("1.0097419586828951E-28")]
    [InlineData("1.0032913020226237E+205")]
    [InlineData("2.2250738585072014E-308")]
    [InlineData("1.7976931348623158E+308")]
    private void ShouldCreateEquivalentRepresentationForDoublePrecision(string value)
    {
        using var solver = PooledSolver.Create();

        var normalFloat = new NormalFloat(Size.FromBits(64U), value);

        using var expr = normalFloat.AsFloat(solver);
        using var bitVector = solver.Context.MkFPToIEEEBV(expr);
        using var simplified = (BitVecNum) bitVector.Simplify();

        var actual = BitConverter.GetBytes(simplified.UInt64);
        var expected = BitConverter.GetBytes(double.Parse(value));

        actual.Should().BeEquivalentTo(expected);
    }
}
