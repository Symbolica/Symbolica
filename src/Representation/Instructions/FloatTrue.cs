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

    public void Execute(IExpressionFactory exprFactory, IState state)
    {
        var result = exprFactory.CreateConstant(Bits.One, BigInteger.One);

        state.Stack.SetVariable(Id, result);
    }
}
