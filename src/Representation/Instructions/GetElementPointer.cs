using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

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
        if (Id == (InstructionId) 111343) // This geezer creates the block (make sure it's got the correct type information all the way down)
            Debugger.Break();
        state.Stack.SetVariable(Id, Address.Create(state.Space, _indexedType, _operands[0].Evaluate(state), GetOffsets(state)));
    }

    private IEnumerable<(IType, IExpression)> GetOffsets(IState state)
    {
        var indexedType = _indexedType;

        foreach (var operand in _operands.Skip(1))
        {
            var index = operand.Evaluate(state);
            var elementType = indexedType.GetType(state.Space, index);
            yield return (elementType, indexedType.GetOffsetBytes(state.Space, index));
            indexedType = elementType;
        }
    }
}
