using System.Numerics;
using Moq;
using Symbolica.Expression;

namespace Symbolica.Representation;

internal static class MockExpression
{
    public static IExpression Create(Nibble value)
    {
        var mockSpace = new Mock<ISpace>(MockBehavior.Strict);

        var mockExpression = new Mock<IExpression>(MockBehavior.Strict);
        mockExpression
            .SetupGet(e => e.Size)
            .Returns(Nibble.Size);
        mockExpression
            .Setup(e => e.GetSingleValue(It.IsAny<ISpace>()))
            .Returns((BigInteger) value);
        mockExpression
            .Setup(e => e.Add(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(value + (Nibble) e.GetSingleValue(mockSpace.Object)));
        mockExpression
            .Setup(e => e.And(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(value & (Nibble) e.GetSingleValue(mockSpace.Object)));
        mockExpression
            .Setup(e => e.ArithmeticShiftRight(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(value.ArithmeticShiftRight((Nibble) e.GetSingleValue(mockSpace.Object))));
        mockExpression
            .Setup(e => e.LogicalShiftRight(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(value.LogicalShiftRight((Nibble) e.GetSingleValue(mockSpace.Object))));
        mockExpression
            .Setup(e => e.Or(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(value | (Nibble) e.GetSingleValue(mockSpace.Object)));
        mockExpression
            .Setup(e => e.ShiftLeft(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(value.ShiftLeft((Nibble) e.GetSingleValue(mockSpace.Object))));
        mockExpression
            .Setup(e => e.Subtract(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(value - (Nibble) e.GetSingleValue(mockSpace.Object)));
        mockExpression
            .Setup(e => e.UnsignedRemainder(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(value.UnsignedRemainder((Nibble) e.GetSingleValue(mockSpace.Object))));
        mockExpression
            .Setup(e => e.Xor(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(value ^ (Nibble) e.GetSingleValue(mockSpace.Object)));

        return mockExpression.Object;
    }
}
