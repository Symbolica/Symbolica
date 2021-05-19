namespace Symbolica.Implementation.System
{
    public interface IDirectory
    {
        long LastAccessTime { get; }
        long LastModifiedTime { get; }

        string[] GetNames();
    }
}