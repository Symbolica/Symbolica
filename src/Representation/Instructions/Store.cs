using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class Store : IInstruction
{
    private readonly IOperand[] _operands;

    public Store(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IExpressionFactory exprFactory, IState state)
    {
        var value = _operands[0].Evaluate(exprFactory, state);
        var address = _operands[1].Evaluate(exprFactory, state);

        state.Memory.Write(address, value);
    }
}
