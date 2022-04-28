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
        if (Id == (InstructionId) 91516)
            Debugger.Break();

        state.Stack.SetVariable(Id, Address.Create(_indexedType, _operands[0].Evaluate(state), GetOffsets(state)));
    }

    private IEnumerable<IExpression> GetOffsets(IState state)
    {
        var indexedType = _indexedType;

        foreach (var operand in _operands.Skip(1))
        {
            var index = operand.Evaluate(state);
            yield return indexedType.GetOffsetBytes(state.Space, index);
            indexedType = indexedType.GetType(state.Space, index);
        }
    }
}
