using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands
{
    public sealed class BlockLabel : IOperand
    {
        private readonly BasicBlockId _basicBlockId;

        public BlockLabel(BasicBlockId basicBlockId)
        {
            _basicBlockId = basicBlockId;
        }

        public IExpression Evaluate(IState state)
        {
            return state.Space.CreateConstant(state.Space.PointerSize, (ulong) _basicBlockId);
        }
    }
}
