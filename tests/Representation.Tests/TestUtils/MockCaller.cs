using Moq;
using Symbolica.Abstraction;

namespace Symbolica.Representation.TestUtils
{
    internal static class MockCaller
    {
        public static ICaller Create(InstructionId instructionId)
        {
            var mockCaller = new Mock<ICaller>(MockBehavior.Strict);
            mockCaller
                .SetupGet(c => c.Id)
                .Returns(instructionId);

            return mockCaller.Object;
        }
    }
}
