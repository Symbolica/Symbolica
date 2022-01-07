using System.IO;

namespace Symbolica.Implementation.System
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
