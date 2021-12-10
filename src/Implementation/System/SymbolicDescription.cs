using System;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal sealed class SymbolicDescription : IPersistentDescription
{
    private readonly ulong _size;
    private readonly long _offset;

    private SymbolicDescription(ulong size, long offset)
    {
        _size = size;
        _offset = offset;
    }

    public (long, IPersistentDescription) Seek(long offset, uint whence)
    {
        var result = whence switch
        {
            0U => offset,
            1U => _offset + offset,
            2U => (long) _size + offset,
            _ => -1L
        };

        return (result, new SymbolicDescription(_size, result));
    }

    public int Read(ISpace space, IMemory memory, IExpression address, int count)
    {
        if (_offset < 0)
            throw new StateException(StateError.InvalidFileOffset, space);

        var size = ((Bytes) (Math.Min(_size - (ulong) _offset, (ulong) count))).ToBits();

        if (size != Bits.Zero)
            memory.Write(address, space.CreateSymbolic(size, null, Enumerable.Empty<Func<IExpression, IExpression>>()));

        return (int) (uint) size;
    }

    public IExpression ReadDirectory(ISpace space, IMemory memory, IStruct entry, IExpression address, int tell)
    {
        return space.CreateConstant(space.PointerSize, BigInteger.Zero);
    }

    public int GetStatus(ISpace space, IMemory memory, IStruct stat, IExpression address)
    {
        // TODO: Should these be symbolic values?
        var type = Convert.ToInt32("0100000", 8);
        var mode = Convert.ToInt32("00444", 8);

        var longSize = ((Bytes) sizeof(long)).ToBits();
        // TODO: Are these offsets still correct?
        memory.Write(address, stat
            .Write(space, 3, type | mode)
            .Write(space, 8, _size)
            .Write(space, 11, space.CreateSymbolic(longSize, null, Enumerable.Empty<Func<IExpression, IExpression>>()))
            .Write(space, 12, space.CreateSymbolic(longSize, null, Enumerable.Empty<Func<IExpression, IExpression>>()))
            .Write(space, 13, space.CreateSymbolic(longSize, null, Enumerable.Empty<Func<IExpression, IExpression>>()))
            .Expression);

        return 0;
    }

    public static IPersistentDescription Create(ulong size)
    {
        return new SymbolicDescription(size, 0L);
    }
}
