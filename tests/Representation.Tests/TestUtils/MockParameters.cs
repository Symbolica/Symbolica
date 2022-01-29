using Moq;
using Symbolica.Abstraction;

namespace Symbolica.Representation.TestUtils;

internal static class MockParameters
{
    public static IParameters Create()
    {
        var mockParameters = new Mock<IParameters>(MockBehavior.Strict);

        return mockParameters.Object;
    }
}
