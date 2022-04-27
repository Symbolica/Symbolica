using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Types;

public sealed class StructType : IStructType
{
    private readonly Bytes[] _offsets;
    private readonly IType[] _types;

    public StructType(Bytes size, Bytes[] offsets, IType[] types)
    {
        Size = size;
        _offsets = offsets;
        _types = types;
    }

    public Bytes Size { get; }
    public IEnumerable<IType> Types => _types;
    public IEnumerable<Bytes> Offsets => _offsets;

    public IType GetType(ISpace space, IExpression index)
    {
        return _types[(int) index.GetSingleValue(space)];
    }

    public IExpression GetOffsetBits(ISpace space, IExpression index)
    {
        return space.CreateConstant(space.PointerSize, (uint) _offsets[(int) index.GetSingleValue(space)].ToBits());
    }

    public IExpression GetOffsetBytes(ISpace space, IExpression index)
    {
        return space.CreateConstant(space.PointerSize, (uint) _offsets[(int) index.GetSingleValue(space)]);
    }

    public IStruct CreateStruct(Func<Bits, IExpression> initializer)
    {
        var size = Size.ToBits();
        var offsets = _offsets.Select(o => o.ToBits()).ToArray();

        var sizes = offsets
            .Skip(1)
            .Append(size)
            .Zip(offsets, (h, l) => h - l)
            .ToArray();

        return new Struct(offsets, sizes,
            initializer(size));
    }
}
