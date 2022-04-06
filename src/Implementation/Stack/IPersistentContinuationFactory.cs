using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentContinuationFactory
{
    (IExpression<IType>, IPersistentContinuationFactory) Create(Bits size);
}
