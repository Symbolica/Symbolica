using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal sealed class SymbolicContinuationFactory : IPersistentContinuationFactory
{
    private readonly IExpressionFactory _exprFactory;

    public SymbolicContinuationFactory(IExpressionFactory exprFactory)
    {
        _exprFactory = exprFactory;
    }

    public (IExpression, IPersistentContinuationFactory) Create(Bits size)
    {
        var continuation = _exprFactory.CreateSymbolic(size, null);

        return (continuation, this);
    }
}
