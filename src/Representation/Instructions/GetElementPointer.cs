using System.Linq;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions;

public sealed class GetElementPointer : IInstruction
{
    private readonly IType _indexedType;
    private readonly IOperand[] _operands;

    public GetElementPointer(InstructionId id, IOperand[] operands, IType indexedType)
    {
        Id = id;
        _operands = operands;
        _indexedType = indexedType;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var address = _operands[0].Evaluate(state);
        var indexedType = _indexedType;

        foreach (var operand in _operands.Skip(1))
        {
            var index = operand.Evaluate(state);
            address = address.Add(indexedType.GetOffsetBytes(state.Space, index));
            indexedType = indexedType.GetType(state.Space, index);
        }

        state.Stack.SetVariable(Id, address);
    }
}
