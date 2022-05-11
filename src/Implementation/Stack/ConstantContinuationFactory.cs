using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal sealed class ConstantContinuationFactory : IPersistentContinuationFactory
{
    private readonly IExpressionFactory _exprFactory;
    private readonly ulong _count;

    private ConstantContinuationFactory(IExpressionFactory exprFactory, ulong count)
    {
        _exprFactory = exprFactory;
        _count = count;
    }

    public (IExpression, IPersistentContinuationFactory) Create(Bits size)
    {
        return (_exprFactory.CreateConstant(size, _count), new ConstantContinuationFactory(_exprFactory, _count + 1UL));
    }

    public static IPersistentContinuationFactory Create(IExpressionFactory exprFactory)
    {
        return new ConstantContinuationFactory(exprFactory, 1UL);
    }
}
