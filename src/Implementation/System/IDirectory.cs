using Symbolica.Expression;

namespace Symbolica.Implementation.System;

public interface IDirectory : IEquivalent<ExpressionSubs, IDirectory>, IMergeable<IDirectory>
{
    long LastAccessTime { get; }
    long LastModifiedTime { get; }
    string[] Names { get; }
}
