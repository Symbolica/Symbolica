using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    [Serializable]
    public class OverlappingMemoryCopyException : StateException
    {
        public OverlappingMemoryCopyException(ISpace space)
            : base(space)
        {
        }
    }
}
