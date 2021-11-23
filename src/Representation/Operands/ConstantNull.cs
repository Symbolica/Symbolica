using System;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands
{
    [Serializable]
    public sealed class ConstantNull : IOperand
    {
        public IExpression Evaluate(IState state)
        {
            return state.Space.CreateConstant(state.Space.PointerSize, BigInteger.Zero);
        }
    }
}
