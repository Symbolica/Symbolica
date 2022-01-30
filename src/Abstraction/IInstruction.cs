namespace Symbolica.Abstraction;

public interface IInstruction
{
    InstructionId Id { get; }

    void Execute(IState state);
}
