using System;
using System.Collections.Generic;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal sealed class FileDescription : IPersistentDescription
{
    private readonly IExpressionFactory _exprFactory;
    private readonly IFile _file;
    private readonly long _offset;

    private FileDescription(IExpressionFactory exprFactory, IFile file, long offset)
    {
        _exprFactory = exprFactory;
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

        return (result, new FileDescription(_exprFactory, _file, result));
    }

    public int Read(ISpace space, IMemory memory, IExpression address, int count)
    {
        var bytes = new byte[count];
        var result = _file.Read(bytes, _offset, count);

        var size = ((Bytes) (uint) result).ToBits();

        if (size != Bits.Zero)
            memory.Write(space, address, _exprFactory.CreateConstant(size, new BigInteger(bytes, true)));

        return result;
    }

    public IExpression ReadDirectory(ISpace space, IMemory memory, IStruct entry, IExpression address, int tell)
    {
        return _exprFactory.CreateZero(_exprFactory.PointerSize);
    }

    public int GetStatus(ISpace space, IMemory memory, IStruct stat, IExpression address)
    {
        var type = Convert.ToInt32("0100000", 8);
        var mode = Convert.ToInt32("00444", 8);

        memory.Write(space, address, stat
            .Write(space, 3, type | mode)
            .Write(space, 8, _file.Size)
            .Write(space, 11, _file.LastAccessTime)
            .Write(space, 12, _file.LastModifiedTime)
            .Write(space, 13, _file.LastModifiedTime)
            .Expression);

        return 0;
    }

    public static IPersistentDescription Create(IExpressionFactory exprFactory, IFile file)
    {
        return new FileDescription(exprFactory, file, 0L);
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(IPersistentDescription other)
    {
        return other is FileDescription fd
            ? _file.IsEquivalentTo(fd._file)
                .And((new(), _offset == fd._offset))
            : (new(), false);
    }

    public object ToJson()
    {
        return new
        {
            File = _file.ToJson(),
            Offset = _offset
        };
    }

    public int GetEquivalencyHash(bool includeSubs)
    {
        return HashCode.Combine(_file.GetEquivalencyHash(includeSubs), _offset);
    }
}
