using System;
using System.IO;
using System.Linq;

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

    private string[] GetNames()
    {
        return _directory.EnumerateFileSystemInfos()
            .Select(i => i.Name)
            .Append(".")
            .Append("..")
            .ToArray();
    }
}
