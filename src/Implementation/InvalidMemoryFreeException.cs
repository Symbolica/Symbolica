using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation
{
    [Serializable]
    public class InvalidMemoryFreeException : StateException
    {
        public InvalidMemoryFreeException(ISpace space)
            : base(space)
        {
        }
    }
}
