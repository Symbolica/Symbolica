using System.IO;
using Symbolica.Implementation.System;

namespace Symbolica.Application.Implementation
{
    internal sealed class FileSystem : IFileSystem
    {
        public IFile? GetFile(string path)
        {
            return File.Exists(path)
                ? new FileProxy(new FileInfo(path))
                : null;
        }

        public IDirectory? GetDirectory(string path)
        {
            return Directory.Exists(path)
                ? new DirectoryProxy(new DirectoryInfo(path))
                : null;
        }
    }
}