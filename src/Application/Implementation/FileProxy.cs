using System;
using System.IO;
using Symbolica.Implementation.System;

namespace Symbolica.Application.Implementation
{
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

        public int Read(byte[] bytes, long offset, int count)
        {
            using var stream = _file.OpenRead();
            stream.Seek(offset, SeekOrigin.Begin);

            return stream.Read(bytes, 0, count);
        }
    }
}