using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class WriteReader : IReader
    {
        private readonly IReader _defaultReader;
        private readonly IExpression _writeBuffer;
        private readonly IExpression _writeMask;
        private readonly IExpression _writeValue;

        public WriteReader(IReader defaultReader,
            IExpression writeBuffer, IExpression writeMask, IExpression writeValue)
        {
            _defaultReader = defaultReader;
            _writeBuffer = writeBuffer;
            _writeMask = writeMask;
            _writeValue = writeValue;
        }

        public IExpression Read(ISymbolicExpression buffer, IExpression offset, Bits size)
        {
            var mask = buffer.Mask(offset, size);

            return mask is ConstantExpression && _writeMask is ConstantExpression
                ? mask.And(_writeMask).Constant.IsZero
                    ? _writeBuffer.Read(offset, size)
                    : mask.Xor(_writeMask).Constant.IsZero
                        ? _writeValue
                        : _defaultReader.Read(buffer, offset, size)
                : _defaultReader.Read(buffer, offset, size);
        }
    }
}
