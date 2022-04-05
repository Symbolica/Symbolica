using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class FloatTrue : IInstruction
{
    public FloatTrue(InstructionId id)
    {
        Id = id;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var result = state.Space.CreateConstant(Size.Bit, BigInteger.One);

        state.Stack.SetVariable(Id, result);
    }
}
