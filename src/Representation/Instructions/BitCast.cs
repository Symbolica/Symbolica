using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Types;

namespace Symbolica.Representation.Instructions;

public sealed class BitCast : IInstruction
{
    private readonly IType? _indexedType;
    private readonly IOperand[] _operands;

    public BitCast(InstructionId id, IOperand[] operands, IType? indexedType)
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
            : Address.Create(new ArrayType(1U, _indexedType), result, Enumerable.Empty<IExpression>()));
    }
}
