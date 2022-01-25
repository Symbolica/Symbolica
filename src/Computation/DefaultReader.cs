using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class DefaultReader : IReader
    {
        public IExpression Read(IExpression buffer, IExpression offset, Bits size)
        {
            return buffer.LogicalShiftRight(offset).Truncate(size);
        }
    }
}
