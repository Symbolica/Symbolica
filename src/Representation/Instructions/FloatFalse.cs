using System.Numerics;
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
        var result = state.Space.CreateConstant(Bits.One, BigInteger.Zero);

        state.Stack.SetVariable(Id, result);
    }
}
