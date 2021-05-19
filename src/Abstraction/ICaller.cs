using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    public interface ICaller
    {
        InstructionId Id { get; }
        Bits Size { get; }
        IAttributes Attributes { get; }

        void Return(IState state);
    }
}