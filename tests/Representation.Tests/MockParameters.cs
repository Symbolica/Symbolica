using Moq;
using Symbolica.Abstraction;

namespace Symbolica.Representation;

internal static class MockParameters
{
    public static IParameters Create()
    {
        var mockParameters = new Mock<IParameters>(MockBehavior.Strict);

        return mockParameters.Object;
    }
}
