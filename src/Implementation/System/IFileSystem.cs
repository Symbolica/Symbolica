namespace Symbolica.Implementation.System;

public interface IFileSystem
{
    IFile? GetFile(string path);
    IDirectory? GetDirectory(string path);
}
