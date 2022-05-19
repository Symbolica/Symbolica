using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Abstraction.Memory;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal sealed class X64VariadicAbi : IVariadicAbi
{
    private readonly IExpressionFactory _exprFactory;

    public X64VariadicAbi(IExpressionFactory exprFactory)
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
            var size = argument.Size.ToBytes();

            if (size > (Bytes) 8U)
                bytes = bytes.AlignTo((Bytes) 16U);

            offsets.Add(bytes.ToBits());
            bytes = (bytes + size).AlignTo((Bytes) 8U);
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
                .Write(space, 0, 48U)
                .Write(space, 1, 304U)
                .Write(space, 2, _address ?? _exprFactory.CreateZero(_exprFactory.PointerSize))
                .Expression;
        }

        public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(IVaList other)
        {
            return other is VaList v
                ? _address is not null && v._address is not null
                    ? _address.IsEquivalentTo(v._address)
                    : (new(), _address is null && v._address is null)
                : (new(), false);
        }

        public object ToJson()
        {
            return new { Address = _address?.ToJson() };
        }
    }
}
