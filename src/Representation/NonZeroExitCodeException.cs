using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    [Serializable]
    public class NonZeroExitCodeException : StateException
    {
        public NonZeroExitCodeException(ISpace space)
            : base(space)
        {
        }
    }
}
