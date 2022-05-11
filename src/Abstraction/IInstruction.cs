using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IInstruction
{
    InstructionId Id { get; }

    void Execute(IExpressionFactory exprFactory, IState state);
}
