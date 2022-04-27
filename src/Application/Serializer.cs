using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Symbolica.Representation;

namespace Symbolica;

internal static class Serializer
{
    public static async Task<byte[]> Serialize(DirectoryInfo directory, string optimization)
    {
        var buildImage = Environment.GetEnvironmentVariable("SYMBOLICA_BUILD_IMAGE");
        var translateImage = Environment.GetEnvironmentVariable("SYMBOLICA_TRANSLATE_IMAGE");

        File.Delete(Path.Combine(directory.FullName, "symbolica.bc"));
        File.Delete(Path.Combine(directory.FullName, ".symbolica.bc"));

        await CallExternalProcess(directory.FullName, buildImage == null
            ? "./symbolica.sh"
            : $"docker run -v $(pwd):/code {buildImage}");

        await CallExternalProcess(directory.FullName, translateImage == null
            ? $"~/.symbolica/translate \"{DeclarationFactory.Pattern}\" {optimization}"
            : $"docker run -v $(pwd):/code {translateImage} \"{DeclarationFactory.Pattern}\" {optimization}");

        return await File.ReadAllBytesAsync(Path.Combine(directory.FullName, ".symbolica.bc"));
    }

    private static async Task CallExternalProcess(string directory, string command)
    {
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        using var process = new Process
        {
            StartInfo =
            {
                WorkingDirectory = directory,
                FileName = isWindows ? "wsl" : "bash",
                Arguments = isWindows ? command : $"-c \"{command.Replace("\"", "\\\"")}\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                //RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        process.Start();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
            throw new Exception($"{await process.StandardError.ReadToEndAsync()}");
    }
}
