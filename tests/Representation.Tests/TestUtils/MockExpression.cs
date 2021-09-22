using System.Numerics;
using Moq;
using Symbolica.Expression;

namespace Symbolica.Representation.TestUtils
{
    internal static class MockExpression
    {
        public static IExpression Create(Nibble value)
        {
            var mockExpression = new Mock<IExpression>(MockBehavior.Strict);
            mockExpression
                .SetupGet(e => e.Size)
                .Returns(Nibble.Size);
            mockExpression
                .SetupGet(e => e.Constant)
                .Returns((BigInteger) value);
            mockExpression
                .Setup(e => e.Add(It.IsAny<IExpression>()))
                .Returns<IExpression>(e => Create(value + (Nibble) e.Constant));
            mockExpression
                .Setup(e => e.And(It.IsAny<IExpression>()))
                .Returns<IExpression>(e => Create(value & (Nibble) e.Constant));
            mockExpression
                .Setup(e => e.ArithmeticShiftRight(It.IsAny<IExpression>()))
                .Returns<IExpression>(e => Create(value.ArithmeticShiftRight((Nibble) e.Constant)));
            mockExpression
                .Setup(e => e.LogicalShiftRight(It.IsAny<IExpression>()))
                .Returns<IExpression>(e => Create(value.LogicalShiftRight((Nibble) e.Constant)));
            mockExpression
                .Setup(e => e.Or(It.IsAny<IExpression>()))
                .Returns<IExpression>(e => Create(value | (Nibble) e.Constant));
            mockExpression
                .Setup(e => e.ShiftLeft(It.IsAny<IExpression>()))
                .Returns<IExpression>(e => Create(value.ShiftLeft((Nibble) e.Constant)));
            mockExpression
                .Setup(e => e.Subtract(It.IsAny<IExpression>()))
                .Returns<IExpression>(e => Create(value - (Nibble) e.Constant));
            mockExpression
                .Setup(e => e.UnsignedRemainder(It.IsAny<IExpression>()))
                .Returns<IExpression>(e => Create(value.UnsignedRemainder((Nibble) e.Constant)));
            mockExpression
                .Setup(e => e.Xor(It.IsAny<IExpression>()))
                .Returns<IExpression>(e => Create(value ^ (Nibble) e.Constant));

            return mockExpression.Object;
        }
    }
}
