using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class IndirectBranch : IInstruction
{
    private readonly IOperand[] _operands;

    public IndirectBranch(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IExpressionFactory exprFactory, IState state)
    {
        var successorId = _operands[0].Evaluate(exprFactory, state);

        state.ForkAll(exprFactory, successorId, new TransferBasicBlock());
    }

    private sealed class TransferBasicBlock : IParameterizedStateAction
    {
        public void Invoke(IState state, BigInteger value)
        {
            state.Stack.TransferBasicBlock((BasicBlockId) (ulong) value);
        }
    }
}
