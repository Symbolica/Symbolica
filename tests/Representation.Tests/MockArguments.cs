using System.Numerics;
using Moq;
using Symbolica.Abstraction;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation;

internal static class MockArguments
{
    public static IArguments Create(params Nibble[] arguments)
    {
        var mockArguments = new Mock<IArguments>(MockBehavior.Strict);
        mockArguments
            .Setup(a => a.Get(It.IsAny<int>()))
            .Returns<int>(i => ConstantUnsigned.Create(Nibble.Size, (BigInteger) arguments[i]));

        return mockArguments.Object;
    }
}
