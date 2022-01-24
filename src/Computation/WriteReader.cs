using Symbolica.Expression;

namespace Symbolica.Computation
{
    internal sealed class WriteReader : IReader
    {
        private readonly IReader _defaultReader;
        private readonly IExpression _previousBuffer;
        private readonly IExpression _previousMask;
        private readonly IExpression _previousValue;

        public WriteReader(IReader defaultReader,
            IExpression previousBuffer, IExpression previousMask, IExpression previousValue)
        {
            _defaultReader = defaultReader;
            _previousBuffer = previousBuffer;
            _previousMask = previousMask;
            _previousValue = previousValue;
        }

        public IExpression Read(ISymbolicExpression buffer, IExpression offset, Bits size)
        {
            var mask = buffer.Mask(offset, size);

            return mask is ConstantExpression && _previousMask is ConstantExpression
                ? mask.And(_previousMask).Constant.IsZero
                    ? _previousBuffer.Read(offset, size)
                    : mask.Xor(_previousMask).Constant.IsZero
                        ? _previousValue
                        : _defaultReader.Read(buffer, offset, size)
                : _defaultReader.Read(buffer, offset, size);
        }
    }
}
