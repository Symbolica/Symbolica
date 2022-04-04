using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public sealed class StructType : IStructType
{
    private readonly Expression.Offset[] _offsets;

    public StructType(Bits size, Expression.Offset[] offsets)
    {
        Size = size;
        _offsets = offsets;
    }

    public Bits Size { get; }

    public Expression.Offset GetOffset(int index)
    {
        return _offsets[index];
    }

    public IStruct CreateStruct(IExpression<IType> expression)
    {
        var offsetSizes = _offsets
            .Select(o => o.FieldSize.ToBits());
        var sizes = offsetSizes
            .Skip(1)
            .Append(Size)
            .Zip(offsetSizes, (h, l) => h - l)
            .ToArray();

        return new Struct(_offsets, sizes, expression);
    }
}
