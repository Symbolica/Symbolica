using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class BitCast : IInstruction
{
    private readonly IOperand[] _operands;
    private readonly Bits _targetSize;

    public BitCast(InstructionId id, IOperand[] operands, Bits targetSize)
    {
        Id = id;
        _operands = operands;
        _targetSize = targetSize;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var value = _operands[0].Evaluate(state);
        var result = value.BitCast(_targetSize);

        state.Stack.SetVariable(Id, result);
    }
}
