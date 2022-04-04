using Symbolica.Abstraction;
using Symbolica.Expression.Values;

namespace Symbolica.Representation.Instructions;

public sealed class InsertValue : IInstruction
{
    private readonly Expression.Offset[] _offsets;
    private readonly IOperand[] _operands;

    public InsertValue(InstructionId id, IOperand[] operands, Expression.Offset[] offsets)
    {
        Id = id;
        _operands = operands;
        _offsets = offsets;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var aggregate = _operands[0].Evaluate(state);
        var value = _operands[1].Evaluate(state);
        var address = Address.CreateNull(state.Space.PointerSize).AppendOffsets(_offsets);
        var result = state.Space.Write(aggregate, address, value);

        state.Stack.SetVariable(Id, result);
    }
}
