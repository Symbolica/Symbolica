using System.Numerics;
using Moq;
using Symbolica.Expression;

namespace Symbolica.Representation;

internal static class MockExpression
{
    public static IExpression Create(ISpace space, Nibble value)
    {
        var mockExpression = new Mock<IExpression>(MockBehavior.Strict);
        mockExpression
            .SetupGet(e => e.Size)
            .Returns(Nibble.Size);
        mockExpression
            .Setup(e => e.GetSingleValue(It.IsAny<ISpace>()))
            .Returns((BigInteger) value);
        mockExpression
            .Setup(e => e.Add(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(space, value + (Nibble) e.GetSingleValue(space)));
        mockExpression
            .Setup(e => e.And(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(space, value & (Nibble) e.GetSingleValue(space)));
        mockExpression
            .Setup(e => e.ArithmeticShiftRight(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(space, value.ArithmeticShiftRight((Nibble) e.GetSingleValue(space))));
        mockExpression
            .Setup(e => e.LogicalShiftRight(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(space, value.LogicalShiftRight((Nibble) e.GetSingleValue(space))));
        mockExpression
            .Setup(e => e.Or(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(space, value | (Nibble) e.GetSingleValue(space)));
        mockExpression
            .Setup(e => e.ShiftLeft(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(space, value.ShiftLeft((Nibble) e.GetSingleValue(space))));
        mockExpression
            .Setup(e => e.Subtract(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(space, value - (Nibble) e.GetSingleValue(space)));
        mockExpression
            .Setup(e => e.UnsignedRemainder(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(space, value.UnsignedRemainder((Nibble) e.GetSingleValue(space))));
        mockExpression
            .Setup(e => e.Xor(It.IsAny<IExpression>()))
            .Returns<IExpression>(e => Create(space, value ^ (Nibble) e.GetSingleValue(space)));

        return mockExpression.Object;
    }
}
