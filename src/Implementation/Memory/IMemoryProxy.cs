using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal interface IMemoryProxy : IMemory, IMergeable<ExpressionSubs, IMemoryProxy>
{
    IMemoryProxy Clone();
}
