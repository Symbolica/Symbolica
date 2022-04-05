using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.Stack;

internal sealed class X64VariadicAbi : IVariadicAbi
{
    public IVaList DefaultVaList => new VaList(null);

    public IVaList PassOnStack(ISpace space, IMemoryProxy memory, IArguments varargs)
    {
        var offsets = new List<Size>();
        var size = Size.Zero;

        foreach (var argument in varargs)
        {
            if (argument.Size > Size.FromBytes(8U))
                size = size.AlignToBytes(Size.FromBytes(16U));

            offsets.Add(size);
            size = (size + argument.Size).AlignToBytes(Size.FromBytes(8U));
        }

        var value = space.CreateZero(size);

        foreach (var (argument, offset) in varargs.Zip(offsets, (a, o) => (a, o)))
            value = value.Write(space.CreateConstant(value.Size, offset.Bits), argument);

        var address = memory.Allocate(Section.Stack, value.Size);
        memory.Write(address, value);

        return new VaList(address);
    }

    private sealed class VaList : IVaList
    {
        private readonly IExpression? _address;

        public VaList(IExpression? address)
        {
            _address = address;
        }

        public IExpression Initialize(ISpace space, IStructType vaListType)
        {
            return vaListType.CreateStruct(space.CreateZero(vaListType.Size))
                .Write(space, 0, 48U)
                .Write(space, 1, 304U)
                .Write(space, 2, _address ?? space.CreateZero(space.PointerSize))
                .Expression;
        }
    }
}
