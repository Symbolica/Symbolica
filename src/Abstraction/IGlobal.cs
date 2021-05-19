using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    public interface IGlobal
    {
        GlobalId Id { get; }
        Bits Size { get; }
        IOperand? Initializer { get; }
    }
}