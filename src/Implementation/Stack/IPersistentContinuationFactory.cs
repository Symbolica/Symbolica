using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentContinuationFactory
{
    (IExpression, IPersistentContinuationFactory) Create(Bits size);
}
