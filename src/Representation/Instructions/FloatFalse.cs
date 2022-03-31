using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

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
        var result = ConstantUnsigned.CreateZero(Bits.One);

        state.Stack.SetVariable(Id, result);
    }
}
