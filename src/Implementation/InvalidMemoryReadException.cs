using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation
{
    [Serializable]
    public class InvalidMemoryReadException : StateException
    {
        public InvalidMemoryReadException(ISpace space)
            : base(space)
        {
        }
    }
}
