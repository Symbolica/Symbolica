using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Implementation.System;

public interface IDirectory : IMergeable<ExpressionSubs, IDirectory>
{
    long LastAccessTime { get; }
    long LastModifiedTime { get; }
    string[] Names { get; }
}
