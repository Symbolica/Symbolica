using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class FloatFalse : IInstruction
{
    public FloatFalse(InstructionId id)
    {
        Id = id;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var result = state.Space.CreateZero(Size.Bit);

        state.Stack.SetVariable(Id, result);
    }
}
