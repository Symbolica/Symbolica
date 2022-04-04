using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Representation.Instructions;

public sealed class ExtractValue : IInstruction
{
    private readonly Expression.Offset[] _offsets;
    private readonly IOperand[] _operands;
    private readonly Bits _size;

    public ExtractValue(InstructionId id, IOperand[] operands, Bits size, Expression.Offset[] offsets)
    {
        Id = id;
        _operands = operands;
        _size = size;
        _offsets = offsets;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var aggregate = _operands[0].Evaluate(state);
        var address = Address.CreateNull(state.Space.PointerSize).AppendOffsets(_offsets);
        var result = state.Space.Read(aggregate, address, _size);

        state.Stack.SetVariable(Id, result);
    }
}
