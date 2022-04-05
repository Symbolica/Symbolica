using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class InsertValue : IInstruction
{
    private readonly Size[] _offsets;
    private readonly IOperand[] _operands;

    public InsertValue(InstructionId id, IOperand[] operands, Size[] offsets)
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
        var offset = state.Space.CreateConstant(aggregate.Size,
            _offsets.Aggregate(Size.Zero, (l, r) => l + r).Bits);
        var result = aggregate.Write(offset, value);

        state.Stack.SetVariable(Id, result);
    }
}
