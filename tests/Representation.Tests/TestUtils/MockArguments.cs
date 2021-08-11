using Moq;
using Symbolica.Abstraction;

namespace Symbolica.Representation.TestUtils
{
    internal static class MockArguments
    {
        public static IArguments Create(params Nibble[] arguments)
        {
            var mockArguments = new Mock<IArguments>(MockBehavior.Strict);
            mockArguments
                .Setup(a => a.Get(It.IsAny<int>()))
                .Returns<int>(i => MockExpression.Create(arguments[i]));

            return mockArguments.Object;
        }
    }
}
