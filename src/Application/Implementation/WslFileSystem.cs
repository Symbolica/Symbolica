using System.IO;
using System.Linq;
using Symbolica.Implementation.System;

namespace Symbolica.Application.Implementation
{
    internal sealed class WslFileSystem : IFileSystem
    {
        private readonly IFileSystem _fileSystem;

        public WslFileSystem(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IFile? GetFile(string path)
        {
            return _fileSystem.GetFile(ToWindowsPath(path));
        }

        public IDirectory? GetDirectory(string path)
        {
            return path == "/mnt"
                ? new WslMountProxy()
                : _fileSystem.GetDirectory(ToWindowsPath(path));
        }

        private static string ToWindowsPath(string path)
        {
            var split = path.Split('/');

            if (split.Length > 2 && split[0] == "" && split[1] == "mnt" && split[2].Length == 1)
                split = split.Skip(3).Prepend($"{split[2]}:").ToArray();

            return string.Join('\\', split);
        }

        private sealed class WslMountProxy : IDirectory
        {
            public long LastAccessTime => 0L;
            public long LastModifiedTime => 0L;

            public string[] GetNames()
            {
                return DriveInfo.GetDrives()
                    .Select(i => i.Name.Substring(0, 1).ToLowerInvariant())
                    .ToArray();
            }
        }
    }
}