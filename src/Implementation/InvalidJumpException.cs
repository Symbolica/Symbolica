using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation
{
    [Serializable]
    public class InvalidJumpException : StateException
    {
        public InvalidJumpException(ISpace space)
            : base(space)
        {
        }
    }
}
