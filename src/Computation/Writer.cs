using System.Numerics;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class Writer : IWriter
    {
        private readonly ICollectionFactory _collectionFactory;
        private readonly IContextFactory _contextFactory;

        public Writer(IContextFactory contextFactory, ICollectionFactory collectionFactory)
        {
            _contextFactory = contextFactory;
            _collectionFactory = collectionFactory;
        }

        public IExpression Mask(IExpression buffer, IExpression offset, Bits size)
        {
            var zero = new ConstantExpression(_contextFactory, _collectionFactory,
                ConstantUnsigned.Create(size, BigInteger.Zero));

            return zero.Not().ZeroExtend(buffer.Size).ShiftLeft(offset);
        }

        public IExpression Write(IExpression buffer, IExpression offset, IExpression value)
        {
            var mask = Mask(buffer, offset, value.Size);

            return buffer.And(mask.Not()).Or(value.ZeroExtend(buffer.Size).ShiftLeft(offset));
        }
    }
}
