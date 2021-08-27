using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.Stack
{
    internal sealed class X64VariadicAbi : IVariadicAbi
    {
        public IExpression PassOnStack(ISpace space, IMemoryProxy memory, IArguments varargs)
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

            var value = space.CreateConstant(bytes.ToBits(), BigInteger.Zero);

            foreach (var (argument, offset) in varargs.Zip(offsets, (a, o) => (a, o)))
                value = value.Write(space.CreateConstant(value.Size, (uint) offset), argument);

            var address = memory.Allocate(Section.Stack, value.Size);
            memory.Write(address, value);

            return address;
        }

        public IExpression CreateVaList(ISpace space, IStructType vaListType, IExpression address)
        {
            return vaListType.CreateStruct(space.CreateConstant(vaListType.Size, BigInteger.Zero))
                .Write(space, 0, 48U)
                .Write(space, 1, 304U)
                .Write(space, 2, address)
                .Expression;
        }
    }
}
