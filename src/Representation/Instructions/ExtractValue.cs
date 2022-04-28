using System.Linq;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions;

public sealed class ExtractValue : IInstruction
{
    private readonly IType _indexedType;
    private readonly IOperand[] _operands;

    public ExtractValue(InstructionId id, IOperand[] operands, IType indexedType)
    {
        Id = id;
        _operands = operands;
        _indexedType = indexedType;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var aggregate = _operands[0].Evaluate(state);
        var offset = state.Space.CreateZero(state.Space.PointerSize);
        var indexedType = _indexedType;

        foreach (var operand in _operands.Skip(1))
        {
            var index = operand.Evaluate(state);
            offset = offset.Add(indexedType.GetOffsetBits(state.Space, index));
            indexedType = indexedType.GetType(state.Space, index);
        }

        var size = indexedType.Size.ToBits();
        var result = aggregate.Read(state.Space, offset.SignExtend(aggregate.Size).Truncate(aggregate.Size), size);

        state.Stack.SetVariable(Id, result);
    }
}
