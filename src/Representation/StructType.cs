using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    public sealed class StructType : IStructType
    {
        private readonly Bits[] _offsets;

        public StructType(Bits size, Bits[] offsets)
        {
            Size = size;
            _offsets = offsets;
        }

        public Bits Size { get; }

        public Bits GetOffset(int index)
        {
            return _offsets[index];
        }

        public IStruct Create(IExpression expression)
        {
            var sizes = _offsets
                .Skip(1)
                .Append(Size)
                .Zip(_offsets, (h, l) => h - l)
                .ToArray();

            return new Struct(_offsets, sizes,
                expression);
        }
    }
}
