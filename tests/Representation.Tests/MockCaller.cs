using Moq;
using Symbolica.Abstraction;

namespace Symbolica.Representation;

internal static class MockCaller
{
    public static ICaller Create(InstructionId id)
    {
        var mockCaller = new Mock<ICaller>(MockBehavior.Strict);
        mockCaller
            .SetupGet(c => c.Id)
            .Returns(id);

        return mockCaller.Object;
    }
}
