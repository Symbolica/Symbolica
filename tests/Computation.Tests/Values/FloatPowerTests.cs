using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatPowerTests
{
    private static readonly IContext Context = DisposableContext.Create();

    [Theory]
    [ClassData(typeof(SinglePowerTestData))]
    [ClassData(typeof(DoublePowerTestData))]
    private void ShouldCreateEquivalentConstants(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatToSigned.Create(constantLeft.Size,
            FloatPower.Create(constantLeft, constantRight)).AsConstant(Context);
        var symbolic = FloatToSigned.Create(symbolicLeft.Size,
            FloatPower.Create(symbolicLeft, symbolicRight)).AsConstant(Context);

        constant.Should().Be(symbolic);
    }

    [Theory]
    [ClassData(typeof(SinglePowerTestData))]
    [ClassData(typeof(DoublePowerTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatToSigned.Create(constantLeft.Size,
            FloatPower.Create(constantLeft, constantRight)).AsBitVector(Context).Simplify();
        var symbolic = FloatToSigned.Create(symbolicLeft.Size,
            FloatPower.Create(symbolicLeft, symbolicRight)).AsBitVector(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SinglePowerTestData))]
    [ClassData(typeof(DoublePowerTestData))]
    private void ShouldCreateEquivalentBooleans(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatToSigned.Create(constantLeft.Size,
            FloatPower.Create(constantLeft, constantRight)).AsBool(Context).Simplify();
        var symbolic = FloatToSigned.Create(symbolicLeft.Size,
            FloatPower.Create(symbolicLeft, symbolicRight)).AsBool(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }

    [Theory]
    [ClassData(typeof(SinglePowerTestData))]
    [ClassData(typeof(DoublePowerTestData))]
    private void ShouldCreateEquivalentFloats(
        IConstantValue constantLeft, IConstantValue constantRight,
        SymbolicFloat symbolicLeft, SymbolicFloat symbolicRight)
    {
        var constant = FloatToSigned.Create(constantLeft.Size,
            FloatPower.Create(constantLeft, constantRight)).AsFloat(Context).Simplify();
        var symbolic = FloatToSigned.Create(symbolicLeft.Size,
            FloatPower.Create(symbolicLeft, symbolicRight)).AsFloat(Context).Simplify();

        constant.Should().BeEquivalentTo(symbolic);
    }
}
