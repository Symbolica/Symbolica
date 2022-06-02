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
    private readonly Lazy<int> _equivalencyHash;

    public StructType(Bytes size, Bytes[] offsets, IType[] types)
    {
        Size = size;
        _offsets = offsets;
        _types = types;
        _equivalencyHash = new(() =>
        {
            var offsetsHash = new HashCode();
            foreach (var offset in _offsets)
                offsetsHash.Add(offset.GetHashCode());

            var typesHash = new HashCode();
            foreach (var type in _types)
                typesHash.Add(type.GetEquivalencyHash());

            return HashCode.Combine(
                offsetsHash.ToHashCode(),
                typesHash.ToHashCode(),
                Size.GetHashCode());
        });
    }

    public Bytes Size { get; }
    public IEnumerable<IType> Types => _types;
    public IEnumerable<Bytes> Offsets => _offsets;

    public IType GetType(ISpace space, IExpression index)
    {
        return _types[(int) index.GetSingleValue(space)];
    }

    public IExpression GetOffsetBits(IExpressionFactory exprFactory, ISpace space, IExpression index)
    {
        return exprFactory.CreateConstant(exprFactory.PointerSize, (uint) _offsets[(int) index.GetSingleValue(space)].ToBits());
    }

    public IExpression GetOffsetBytes(IExpressionFactory exprFactory, ISpace space, IExpression index)
    {
        return exprFactory.CreateConstant(exprFactory.PointerSize, (uint) _offsets[(int) index.GetSingleValue(space)]);
    }

    public IStruct CreateStruct(IExpressionFactory exprFactory, Func<Bits, IExpression> initializer)
    {
        var size = Size.ToBits();
        var offsets = _offsets.Select(o => o.ToBits()).ToArray();

        var sizes = offsets
            .Skip(1)
            .Append(size)
            .Zip(offsets, (h, l) => h - l)
            .ToArray();

        return new Struct(exprFactory, offsets, sizes,
            initializer(size));
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IType other)
    {
        return other is StructType st
            ? _offsets.IsSequenceEquivalentTo<ExpressionSubs, Bytes>(st._offsets, (a, b) => (new(), a == b))
                .And(_types.IsSequenceEquivalentTo<ExpressionSubs, IType>(st._types))
                .And((new(), Size == st.Size))
            : (new(), false);
    }

    public object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Offsets = _offsets.Select(o => (uint) o).ToArray(),
            Types = _types.Select(t => t.ToJson()).ToArray()
        };
    }

    public int GetEquivalencyHash()
    {
        return _equivalencyHash.Value;
    }
}
