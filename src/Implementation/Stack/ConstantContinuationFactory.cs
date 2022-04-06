using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Implementation.Stack;

internal sealed class ConstantContinuationFactory : IPersistentContinuationFactory
{
    private readonly ulong _count;

    private ConstantContinuationFactory(ulong count)
    {
        _count = count;
    }

    public (IExpression<IType>, IPersistentContinuationFactory) Create(Bits size)
    {
        return (ConstantUnsigned.Create(size, _count), new ConstantContinuationFactory(_count + 1UL));
    }

    public static IPersistentContinuationFactory Create()
    {
        return new ConstantContinuationFactory(1UL);
    }
}
