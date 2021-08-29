using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    [Serializable]
    public class ZeroDivisorException : StateException
    {
        public ZeroDivisorException(ISpace space)
            : base(space)
        {
        }
    }
}
