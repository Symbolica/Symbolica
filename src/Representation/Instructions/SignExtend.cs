using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class SignExtend : IInstruction
{
    private readonly IOperand[] _operands;
    private readonly Size _size;

    public SignExtend(InstructionId id, IOperand[] operands, Size size)
    {
        Id = id;
        _operands = operands;
        _size = size;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var expression = _operands[0].Evaluate(state);
        var result = expression.SignExtend(_size);

        state.Stack.SetVariable(Id, result);
    }
}
