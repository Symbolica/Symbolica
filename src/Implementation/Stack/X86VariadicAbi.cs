using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;
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

        var value = ConstantUnsigned.CreateZero(bytes.ToBits()) as IExpression<IType>;

        foreach (var (argument, offset) in varargs.Zip(offsets, (a, o) => (a, o)))
            value = space.Write(value, ConstantUnsigned.Create(value.Size, (uint) offset), argument);

        var address = memory.Allocate(Section.Stack, value.Size);
        memory.Write(address, value);

        return new VaList(address);
    }

    private sealed class VaList : IVaList
    {
        private readonly IExpression<IType>? _address;

        public VaList(IExpression<IType>? address)
        {
            _address = address;
        }

        public IExpression<IType> Initialize(ISpace space, IStructType vaListType)
        {
            return vaListType.CreateStruct(ConstantUnsigned.CreateZero(vaListType.Size))
                .Write(space, 0, _address ?? ConstantUnsigned.CreateZero(space.PointerSize))
                .Expression;
        }
    }
}
