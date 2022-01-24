using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class Reader : IReader
    {
        private readonly ISymbolicExpression _buffer;
        private readonly IReader _defaultReader;
        private readonly IExpression _mask;
        private readonly IExpression _value;

        public Reader(IReader defaultReader, ISymbolicExpression buffer, IExpression mask, IExpression value)
        {
            _defaultReader = defaultReader;
            _buffer = buffer;
            _mask = mask;
            _value = value;
        }

        public IExpression Read(ISymbolicExpression buffer, IExpression offset, Bits size)
        {
            var mask = buffer.Mask(offset, size);

            return mask is ConstantExpression && _mask is ConstantExpression
                ? mask.And(_mask).Constant.IsZero
                    ? _buffer.Read(offset, size)
                    : mask.Xor(_mask).Constant.IsZero
                        ? _value
                        : _defaultReader.Read(buffer, offset, size)
                : _defaultReader.Read(buffer, offset, size);
        }
    }
}
