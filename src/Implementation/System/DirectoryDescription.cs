using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal sealed class DirectoryDescription : IPersistentDescription
{
    private readonly IExpressionFactory _exprFactory;
    private readonly IDirectory _directory;

    public DirectoryDescription(IExpressionFactory exprFactory, IDirectory directory)
    {
        _exprFactory = exprFactory;
        _directory = directory;
    }

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
        return tell >= 0 && tell < _directory.Names.Length
            ? Read(space, memory, entry, address, _directory.Names[tell])
            : _exprFactory.CreateZero(_exprFactory.PointerSize);
    }

    public int GetStatus(ISpace space, IMemory memory, IStruct stat, IExpression address)
    {
        var type = Convert.ToInt32("0040000", 8);
        var mode = Convert.ToInt32("00444", 8);

        memory.Write(space, address, stat
            .Write(space, 3, type | mode)
            .Write(space, 8, 0L)
            .Write(space, 11, _directory.LastAccessTime)
            .Write(space, 12, _directory.LastModifiedTime)
            .Write(space, 13, _directory.LastModifiedTime)
            .Expression);

        return 0;
    }

    private static IExpression Read(ISpace space, IMemory memory, IStruct entry, IExpression address, string name)
    {
        memory.Write(space, address, entry
            .Write(space, 4, new BigInteger(Encoding.UTF8.GetBytes(name), true))
            .Expression);

        return address;
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IPersistentDescription other)
    {
        return other is DirectoryDescription dd
            ? _directory.IsEquivalentTo(dd._directory)
            : (new(), false);
    }

    public object ToJson()
    {
        return new
        {
            Directory = _directory.ToJson()
        };
    }

    public int GetEquivalencyHash()
    {
        return _directory.GetEquivalencyHash();
    }

    public int GetMergeHash()
    {
        return _directory.GetMergeHash();
    }

    public bool TryMerge(IPersistentDescription other, IExpression predicate, [MaybeNullWhen(false)] out IPersistentDescription merged)
    {
        if (other is DirectoryDescription dd
            && _directory.TryMerge(dd._directory, predicate, out var mergedDirectory))
        {
            merged = new DirectoryDescription(_exprFactory, mergedDirectory);
            return true;
        }

        merged = null;
        return false;
    }
}
