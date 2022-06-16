using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal sealed class DirectoryProxy : IDirectory
{
    private readonly DirectoryInfo _directory;
    private readonly Lazy<string[]> _names;

    public DirectoryProxy(DirectoryInfo directory)
    {
        _directory = directory;
        _names = new Lazy<string[]>(GetNames);
    }

    public long LastAccessTime => new DateTimeOffset(_directory.LastAccessTimeUtc).ToUnixTimeSeconds();
    public long LastModifiedTime => new DateTimeOffset(_directory.LastWriteTimeUtc).ToUnixTimeSeconds();
    public string[] Names => _names.Value;

    public int GetEquivalencyHash()
    {
        return _directory.GetHashCode();
    }

    public int GetMergeHash()
    {
        return _directory.GetHashCode();
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IDirectory other)
    {
        return (new(),
            other is DirectoryProxy dp
            && _directory.Equals(dp._directory)
            && _names.Value.SequenceEqual(dp._names.Value));
    }

    public object ToJson()
    {
        return _directory;
    }

    public bool TryMerge(IDirectory other, IExpression predicate, [MaybeNullWhen(false)] out IDirectory merged)
    {
        merged = this;
        return other is DirectoryProxy dp
            && _directory.Equals(dp._directory)
            && _names.Value.SequenceEqual(dp._names.Value);
    }

    private string[] GetNames()
    {
        return _directory.EnumerateFileSystemInfos()
            .Select(i => i.Name)
            .Append(".")
            .Append("..")
            .ToArray();
    }
}
