using System;
using System.Collections.Generic;
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

    public int GetEquivalencyHash(bool includeSubs)
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

    private string[] GetNames()
    {
        return _directory.EnumerateFileSystemInfos()
            .Select(i => i.Name)
            .Append(".")
            .Append("..")
            .ToArray();
    }
}
