using System;
using FluentAssertions;
using Microsoft.Z3;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation;

public class NormalFloatTests
{
    private static readonly Context Context = new();

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
        var normalFloat = new NormalFloat((Bits) 32U, value);

        var expr = normalFloat.AsFloat(Context);

        var actual = BitConverter.GetBytes(((BitVecNum) Context.MkFPToIEEEBV(expr).Simplify()).UInt);
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
        var normalFloat = new NormalFloat((Bits) 64U, value);

        var expr = normalFloat.AsFloat(Context);

        var actual = BitConverter.GetBytes(((BitVecNum) Context.MkFPToIEEEBV(expr).Simplify()).UInt64);
        var expected = BitConverter.GetBytes(double.Parse(value));

        actual.Should().BeEquivalentTo(expected);
    }
}
