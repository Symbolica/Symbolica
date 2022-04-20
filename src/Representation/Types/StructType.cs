using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Types;

public sealed class StructType : IStructType
{
    private readonly Bits[] _offsets;
    private readonly IType[] _types;

    public StructType(Bits size, Bits[] offsets, IType[] types)
    {
        Size = size;
        _offsets = offsets;
        _types = types;
    }

    public Bits Size { get; }

    public IType GetType(ISpace space, IExpression index)
    {
        return _types[(int) index.GetSingleValue(space)];
    }

    public IExpression GetOffsetBits(ISpace space, IExpression index)
    {
        return space.CreateConstant(space.PointerSize, (uint) _offsets[(int) index.GetSingleValue(space)]);
    }

    public IExpression GetOffsetBytes(ISpace space, IExpression index)
    {
        return space.CreateConstant(space.PointerSize, (uint) _offsets[(int) index.GetSingleValue(space)].ToBytes());
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
