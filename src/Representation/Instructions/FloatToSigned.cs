using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class FloatToSigned : IInstruction
{
    private readonly IOperand[] _operands;
    private readonly Size _size;

    public FloatToSigned(InstructionId id, IOperand[] operands, Size size)
    {
        Id = id;
        _operands = operands;
        _size = size;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var expression = _operands[0].Evaluate(state);
        var result = expression.FloatToSigned(_size);

        state.Stack.SetVariable(Id, result);
    }
}
