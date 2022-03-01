using System.Linq;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions;

public sealed class GetElementPointer : IInstruction
{
    private readonly Offset[] _offsets;
    private readonly IOperand[] _operands;

    public GetElementPointer(InstructionId id, IOperand[] operands, Offset[] offsets)
    {
        Id = id;
        _operands = operands;
        _offsets = offsets;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        Expression.Offset EvaluateOffset(Offset offset)
        {
            return new Expression.Offset(
                offset.AggregateSize,
                offset.AggregateType,
                offset.FieldSize,
                offset.Value.Evaluate(state));
        }

        var baseAddress = _operands[0].Evaluate(state);
        var offsets = _offsets.Select(EvaluateOffset).ToArray();

        var address = state.Space.CreateAddress(baseAddress, offsets);

        state.Stack.SetVariable(Id, address);
    }
}
