using System;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Implementation.System;

internal sealed class OutputDescription : IPersistentDescription
{
    public (long, IPersistentDescription) Seek(long offset, uint whence)
    {
        return (-1L, this);
    }

    public int Read(ISpace space, IMemory memory, IExpression<IType> address, int count)
    {
        return -1;
    }

    public IExpression<IType> ReadDirectory(ISpace space, IMemory memory, IStruct entry, IExpression<IType> address, int tell)
    {
        return ConstantUnsigned.CreateZero(space.PointerSize);
    }

    public int GetStatus(ISpace space, IMemory memory, IStruct stat, IExpression<IType> address)
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
