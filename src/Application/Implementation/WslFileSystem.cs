using System.Diagnostics;
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
            return _fileSystem.GetDirectory(ToWindowsPath(path));
        }

        private static string ToWindowsPath(string path)
        {
            using var process = new Process
            {
                StartInfo =
                {
                    FileName = "wsl",
                    Arguments = $"wslpath -w {path}",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();
            process.WaitForExit();

            return process.StandardOutput.ReadToEnd().Trim();
        }
    }
}
