using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public sealed class StructType : IStructType
{
    private readonly Size[] _offsets;

    public StructType(Size size, Size[] offsets)
    {
        Size = size;
        _offsets = offsets;
    }

    public Size Size { get; }

    public Size GetOffset(int index)
    {
        return _offsets[index];
    }

    public IStruct CreateStruct(IExpression expression)
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
