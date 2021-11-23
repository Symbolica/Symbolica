using System;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System
{
    [Serializable]
    internal sealed class OutputDescription : IPersistentDescription
    {
        public (long, IPersistentDescription) Seek(long offset, uint whence)
        {
            return (-1L, this);
        }

        public int Read(ISpace space, IMemory memory, IExpression address, int count)
        {
            return -1;
        }

        public IExpression ReadDirectory(ISpace space, IMemory memory, IStruct entry, IExpression address, int tell)
        {
            return space.CreateConstant(space.PointerSize, BigInteger.Zero);
        }

        public int GetStatus(ISpace space, IMemory memory, IStruct stat, IExpression address)
        {
            var type = Convert.ToInt32("0010000", 8);
            var mode = Convert.ToInt32("00222", 8);

            memory.Write(address, stat
                .Write(space, 3, type | mode)
                .Write(space, 8, 0L)
                .Expression);

            return 0;
        }
    }
}
