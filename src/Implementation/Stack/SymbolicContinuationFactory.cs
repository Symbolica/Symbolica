using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal sealed class SymbolicContinuationFactory : IPersistentContinuationFactory
{
    public (IExpression<IType>, IPersistentContinuationFactory) Create(Bits size)
    {
        var continuation = Symbol.Create(size);

        return (continuation, this);
    }
}
