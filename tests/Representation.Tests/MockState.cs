using System;
using System.Numerics;
using Moq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

internal static class MockState
{
    public static IState Create(InstructionId instructionId, Action<Nibble> callback)
    {
        var mockSpace = new Mock<ISpace>(MockBehavior.Strict);

        var mockStack = new Mock<IStack>(MockBehavior.Strict);
        mockStack
            .Setup(s => s.SetVariable(instructionId, It.IsAny<IExpression<IType>>()))
            .Callback<InstructionId, IExpression<IType>>((_, v) => callback((Nibble) (BigInteger) (v as IConstantValue<IType>)!.AsUnsigned()));

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
