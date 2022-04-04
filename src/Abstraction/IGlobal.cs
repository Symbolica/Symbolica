using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Abstraction;

public interface IGlobal
{
    GlobalId Id { get; }
    Bits Size { get; }

    void Initialize(IState state, Address address);
}
