using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class FloatTruncate : IInstruction
{
    private readonly IOperand[] _operands;
    private readonly Bits _size;

    public FloatTruncate(InstructionId id, IOperand[] operands, Bits size)
    {
        Id = id;
        _operands = operands;
        _size = size;
    }

    public InstructionId Id { get; }

    public void Execute(IExpressionFactory exprFactory, IState state)
    {
        var expression = _operands[0].Evaluate(exprFactory, state);
        var result = expression.FloatConvert(_size);

        state.Stack.SetVariable(Id, result);
    }
}
