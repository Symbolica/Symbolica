using System.Linq;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions;

public sealed class InsertValue : IInstruction
{
    private readonly IType _indexedType;
    private readonly IOperand[] _operands;

    public InsertValue(InstructionId id, IOperand[] operands, IType indexedType)
    {
        Id = id;
        _operands = operands;
        _indexedType = indexedType;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var aggregate = _operands[0].Evaluate(state);
        var value = _operands[1].Evaluate(state);
        var offset = state.Space.CreateZero(state.Space.PointerSize);
        var indexedType = _indexedType;

        foreach (var operand in _operands.Skip(2))
        {
            var index = operand.Evaluate(state);
            offset = offset.Add(indexedType.GetOffsetBits(state.Space, index));
            indexedType = indexedType.GetType(state.Space, index);
        }

        var result = aggregate.Write(state.Space, offset.SignExtend(aggregate.Size).Truncate(aggregate.Size), value);

        state.Stack.SetVariable(Id, result);
    }
}
