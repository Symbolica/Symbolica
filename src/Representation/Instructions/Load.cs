using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class Load : IInstruction
{
    private readonly IOperand[] _operands;
    private readonly Bits _size;

    public Load(InstructionId id, IOperand[] operands, Bits size)
    {
        Id = id;
        _operands = operands;
        _size = size;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var address = _operands[0].Evaluate(state);
        var value = state.Memory.Read(address, _size);

        // TODO: When loading an address it should be bitcast to the size being pointed to

        state.Stack.SetVariable(Id, value);
    }
}
