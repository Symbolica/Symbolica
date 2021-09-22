using System;
using System.Numerics;
using Moq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.TestUtils
{
    internal static class MockState
    {
        public static IState Create(InstructionId instructionId, Action<Nibble> callback)
        {
            var mockSpace = new Mock<ISpace>(MockBehavior.Strict);
            mockSpace
                .Setup(s => s.CreateConstant(Nibble.Size, It.IsAny<BigInteger>()))
                .Returns<Bits, BigInteger>((_, v) => MockExpression.Create((Nibble) v));

            var mockStack = new Mock<IStack>(MockBehavior.Strict);
            mockStack
                .Setup(s => s.SetVariable(instructionId, It.IsAny<IExpression>()))
                .Callback<InstructionId, IExpression>((_, e) => callback((Nibble) e.Constant));

            var mockState = new Mock<IState>(MockBehavior.Strict);
            mockState
                .SetupGet(s => s.Space)
                .Returns(mockSpace.Object);
            mockState
                .SetupGet(s => s.Stack)
                .Returns(mockStack.Object);

            return mockState.Object;
        }
    }
}
