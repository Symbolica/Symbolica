using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class BitCast : IInstruction
{
    private readonly IArrayType? _indexedType;
    private readonly IOperand[] _operands;

    public BitCast(InstructionId id, IOperand[] operands, IArrayType? indexedType)
    {
        Id = id;
        _operands = operands;
        _indexedType = indexedType;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var result = _operands[0].Evaluate(state);
        state.Stack.SetVariable(Id, _indexedType == null
            ? result
            : result is IAddress a
                ? a.Offsets.Any()
                    ? a
                    : Address.Create(state.Space, _indexedType, a.BaseAddress, a.Offsets)
                : Address.Create(state.Space, _indexedType, result, Enumerable.Empty<(IType, IExpression)>()));
    }
}
