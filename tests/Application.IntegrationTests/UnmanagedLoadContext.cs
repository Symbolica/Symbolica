using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;

namespace Symbolica.Application
{
    internal sealed class UnmanagedLoadContext
    {
        private readonly ConcurrentBag<string> _directories;
        private readonly ConcurrentDictionary<string, IntPtr> _handles;

        public UnmanagedLoadContext()
        {
            _directories = new ConcurrentBag<string>();
            _handles = new ConcurrentDictionary<string, IntPtr>();
        }

        public void Unload()
        {
            foreach (var handle in _handles.Values)
                NativeLibrary.Free(handle);

            foreach (var directory in _directories)
                Directory.Delete(directory, true);
        }

        public IntPtr LoadUnmanagedDll(string libraryPath)
        {
            return _handles.GetOrAdd(libraryPath, Load);
        }

        private IntPtr Load(string libraryPath)
        {
            var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory);
            _directories.Add(directory);

            var tempPath = Path.Combine(directory, Path.GetFileName(libraryPath));
            File.Copy(libraryPath, tempPath);

            return NativeLibrary.Load(tempPath);
        }
    }
}
