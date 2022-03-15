using Moq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

internal static class MockArguments
{
    public static IArguments Create(ISpace space, params Nibble[] arguments)
    {
        var mockArguments = new Mock<IArguments>(MockBehavior.Strict);
        mockArguments
            .Setup(a => a.Get(It.IsAny<int>()))
            .Returns<int>(i => MockExpression.Create(space, arguments[i]));

        return mockArguments.Object;
    }
}
