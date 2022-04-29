using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions;

public sealed class BitCast : IInstruction
{
    private readonly IPointerType? _indexedType;
    private readonly IOperand[] _operands;

    public BitCast(InstructionId id, IOperand[] operands, IPointerType? indexedType)
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
                ? a
                : Address.Create(state.Space, _indexedType, result, new[] { (_indexedType.ElementType, state.Space.CreateConstant(state.Space.PointerSize, 0U)) }));
    }
}
