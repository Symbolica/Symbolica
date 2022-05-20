using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Abstraction.Memory;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal sealed class X86VariadicAbi : IVariadicAbi
{
    private readonly IExpressionFactory _exprFactory;

    public X86VariadicAbi(IExpressionFactory exprFactory)
    {
        _exprFactory = exprFactory;
    }

    public IVaList DefaultVaList => new VaList(_exprFactory, null);

    public IVaList PassOnStack(ISpace space, IMemory memory, IArguments varargs)
    {
        var offsets = new List<Bits>();
        var bytes = Bytes.Zero;

        foreach (var argument in varargs)
        {
            offsets.Add(bytes.ToBits());
            bytes = (bytes + argument.Size.ToBytes()).AlignTo(Bytes.One);
        }

        var value = _exprFactory.CreateZero(bytes.ToBits());

        foreach (var (argument, offset) in varargs.Zip(offsets, (a, o) => (a, o)))
            value = value.Write(space, _exprFactory.CreateConstant(value.Size, (uint) offset), argument);

        var address = memory.Allocate(Section.Stack, value.Size);
        memory.Write(space, address, value);

        return new VaList(_exprFactory, address);
    }

    private sealed class VaList : IVaList
    {
        private readonly IExpressionFactory _exprFactory;
        private readonly IExpression? _address;

        public VaList(IExpressionFactory exprFactory, IExpression? address)
        {
            _exprFactory = exprFactory;
            _address = address;
        }

        public IExpression Initialize(ISpace space, IStructType vaListType)
        {
            return vaListType.CreateStruct(_exprFactory, _exprFactory.CreateZero)
                .Write(space, 0, _address ?? _exprFactory.CreateZero(_exprFactory.PointerSize))
                .Expression;
        }

        public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(IVaList other)
        {
            return other is VaList v
                ? Mergeable.IsNullableEquivalentTo<IExpression, IExpression>(_address, v._address)
                : (new(), false);
        }

        public int GetEquivalencyHash()
        {
            return HashCode.Combine(_address?.GetEquivalencyHash());
        }

        public object ToJson()
        {
            return new { Address = _address?.ToJson() };
        }
    }
}
