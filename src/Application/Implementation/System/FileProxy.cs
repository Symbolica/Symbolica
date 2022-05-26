using System;
using System.Collections.Generic;
using System.IO;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

internal sealed class FileProxy : IFile
{
    private readonly FileInfo _file;

    public FileProxy(FileInfo file)
    {
        _file = file;
    }

    public long LastAccessTime => new DateTimeOffset(_file.LastAccessTimeUtc).ToUnixTimeSeconds();
    public long LastModifiedTime => new DateTimeOffset(_file.LastWriteTimeUtc).ToUnixTimeSeconds();
    public long Size => _file.Length;

    public int GetEquivalencyHash(bool includeSubs)
    {
        return _file.GetHashCode();
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IFile other)
    {
        // TODO: This might just be using reference equality on FileInfo
        return (new(), other is FileProxy fp && _file.Equals(fp._file));
    }

    public int Read(byte[] bytes, long offset, int count)
    {
        using var stream = _file.OpenRead();
        stream.Seek(offset, SeekOrigin.Begin);

        return stream.Read(bytes, 0, count);
    }

    public object ToJson()
    {
        return _file;
    }
}
