using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface ICaller
{
    InstructionId Id { get; }
    Size Size { get; }
    IAttributes ReturnAttributes { get; }

    void Return(IState state);
}
