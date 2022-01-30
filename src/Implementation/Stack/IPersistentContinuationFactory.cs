using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentContinuationFactory
{
    (IExpression, IPersistentContinuationFactory) Create(ISpace space, Bits size);
}
