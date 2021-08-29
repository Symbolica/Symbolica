using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation
{
    [Serializable]
    public class InvalidMemoryWriteException : StateException
    {
        public InvalidMemoryWriteException(ISpace space)
            : base(space)
        {
        }
    }
}
