using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class GetElementPointer : IInstruction
{
    private readonly (Bytes, IOperand)[] _offsets;
    private readonly IOperand[] _operands;

    public GetElementPointer(InstructionId id, IOperand[] operands, (Bytes, IOperand)[] offsets)
    {
        Id = id;
        _operands = operands;
        _offsets = offsets;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var baseAddress = _operands[0].Evaluate(state);
        var offsets = _offsets.Select((o) => (o.Item1, o.Item2.Evaluate(state))).ToArray();

        var aggregateOffset = state.Space.CreateAggregateOffset(baseAddress, offsets);

        state.Stack.SetVariable(Id, aggregateOffset);
    }
}
