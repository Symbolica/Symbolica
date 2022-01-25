using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class WriteReader : IReader
    {
        private readonly IReader _defaultReader;
        private readonly IExpression _writeBuffer;
        private readonly IExpression _writeOffset;
        private readonly IWriter _writer;
        private readonly IExpression _writeValue;

        public WriteReader(IWriter writer, IReader defaultReader,
            IExpression writeBuffer, IExpression writeOffset, IExpression writeValue)
        {
            _writer = writer;
            _defaultReader = defaultReader;
            _writeBuffer = writeBuffer;
            _writeOffset = writeOffset;
            _writeValue = writeValue;
        }

        public IExpression Read(IExpression buffer, IExpression offset, Bits size)
        {
            var mask = _writer.Mask(buffer, offset, size);
            var writeMask = _writer.Mask(_writeBuffer, _writeOffset, _writeValue.Size);

            return mask is ConstantExpression && writeMask is ConstantExpression
                ? mask.And(writeMask).Constant.IsZero
                    ? _writeBuffer.Read(offset, size)
                    : mask.Xor(writeMask).Constant.IsZero
                        ? _writeValue
                        : _defaultReader.Read(buffer, offset, size)
                : _defaultReader.Read(buffer, offset, size);
        }
    }
}
