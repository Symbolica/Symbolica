using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation
{
    [Serializable]
    public class InvalidMemoryMoveException : StateException
    {
        public InvalidMemoryMoveException(ISpace space)
            : base(space)
        {
        }
    }
}
