using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IGlobal
{
    GlobalId Id { get; }
    Size Size { get; }

    void Initialize(IState state, IExpression address);
}
