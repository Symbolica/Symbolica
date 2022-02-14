using Moq;

namespace Symbolica.Computation;

internal static class MockAssertions
{
    public static IAssertions Create()
    {
        var mockAssertions = new Mock<IAssertions>(MockBehavior.Strict);

        return mockAssertions.Object;
    }
}
