using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.Stack;

internal sealed class X86VariadicAbi : IVariadicAbi
{
    public IVaList DefaultVaList => new VaList(null);

    public IVaList PassOnStack(ISpace space, IMemoryProxy memory, IArguments varargs)
    {
        var offsets = new List<Bits>();
        var bytes = Bytes.Zero;

        foreach (var argument in varargs)
        {
            offsets.Add(bytes.ToBits());
            bytes = (bytes + argument.Size.ToBytes()).AlignTo(Bytes.One);
        }

        var value = space.CreateZero(bytes.ToBits());

        foreach (var (argument, offset) in varargs.Zip(offsets, (a, o) => (a, o)))
            value = value.Write(space, space.CreateConstant(value.Size, (uint) offset), argument);

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
                .Write(space, 0, _address ?? space.CreateZero(space.PointerSize))
                .Expression;
        }
    }
}
