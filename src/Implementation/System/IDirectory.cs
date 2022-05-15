using Symbolica.Expression;

namespace Symbolica.Implementation.System;

public interface IDirectory : IMergeable<IExpression, IDirectory>
{
    long LastAccessTime { get; }
    long LastModifiedTime { get; }
    string[] Names { get; }
}
