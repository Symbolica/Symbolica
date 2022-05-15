using Symbolica.Expression;

namespace Symbolica.Implementation.System;

public interface IFile : IMergeable<IExpression, IFile>
{
    long LastAccessTime { get; }
    long LastModifiedTime { get; }
    long Size { get; }

    int Read(byte[] bytes, long offset, int count);
}
