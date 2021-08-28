using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands
{
    public sealed class BlockLabel : IOperand
    {
        private readonly BasicBlockId _id;

        public BlockLabel(BasicBlockId id)
        {
            _id = id;
        }

        public IExpression Evaluate(IState state)
        {
            return state.Space.CreateConstant(state.Space.PointerSize, (ulong) _id);
        }
    }
}
