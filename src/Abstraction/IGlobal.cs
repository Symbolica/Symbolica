using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IGlobal
{
    GlobalId Id { get; }
    Bits Size { get; }

    void Initialize(IExpressionFactory exprFactory, IState state, IExpression address);
}
