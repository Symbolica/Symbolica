using System;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Implementation.System;

internal sealed class FileDescription : IPersistentDescription
{
    private readonly IFile _file;
    private readonly long _offset;

    private FileDescription(IFile file, long offset)
    {
        _file = file;
        _offset = offset;
    }

    public (long, IPersistentDescription) Seek(long offset, uint whence)
    {
        var result = whence switch
        {
            0U => offset,
            1U => _offset + offset,
            2U => _file.Size + offset,
            _ => -1L
        };

        return (result, new FileDescription(_file, result));
    }

    public int Read(ISpace space, IMemory memory, IExpression<IType> address, int count)
    {
        var bytes = new byte[count];
        var result = _file.Read(bytes, _offset, count);

        var size = ((Bytes) (uint) result).ToBits();

        if (size != Bits.Zero)
            memory.Write(address, ConstantUnsigned.Create(size, new BigInteger(bytes, true)));

        return result;
    }

    public IExpression<IType> ReadDirectory(ISpace space, IMemory memory, IStruct entry, IExpression<IType> address, int tell)
    {
        return ConstantUnsigned.CreateZero(space.PointerSize);
    }

    public int GetStatus(ISpace space, IMemory memory, IStruct stat, IExpression<IType> address)
    {
        var type = Convert.ToInt32("0100000", 8);
        var mode = Convert.ToInt32("00444", 8);

        memory.Write(address, stat
            .Write(space, 3, type | mode)
            .Write(space, 8, _file.Size)
            .Write(space, 11, _file.LastAccessTime)
            .Write(space, 12, _file.LastModifiedTime)
            .Write(space, 13, _file.LastModifiedTime)
            .Expression);

        return 0;
    }

    public static IPersistentDescription Create(IFile file)
    {
        return new FileDescription(file, 0L);
    }
}
