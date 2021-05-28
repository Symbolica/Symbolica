using Symbolica.Expression;

namespace Symbolica.Implementation.Stack
{
    internal sealed class ConstantContinuationFactory : IPersistentContinuationFactory
    {
        private readonly ulong _count;

        private ConstantContinuationFactory(ulong count)
        {
            _count = count;
        }

        public (IExpression, IPersistentContinuationFactory) Create(ISpace space, Bits size)
        {
            return (space.CreateConstant(size, _count), new ConstantContinuationFactory(_count + 1UL));
        }

        public static IPersistentContinuationFactory Create()
        {
            return new ConstantContinuationFactory(1UL);
        }
    }
}
