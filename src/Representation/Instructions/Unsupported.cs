using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Exceptions;

namespace Symbolica.Representation.Instructions;

public sealed class Unsupported : IInstruction
{
    private readonly string _type;

    public Unsupported(InstructionId id, string type)
    {
        Id = id;
        _type = type;
    }

    public InstructionId Id { get; }

    public void Execute(IExpressionFactory exprFactory, IState state)
    {
        throw new UnsupportedInstructionException(_type);
    }
}
