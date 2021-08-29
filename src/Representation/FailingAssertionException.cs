using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    [Serializable]
    public class FailingAssertionException : StateException
    {
        public FailingAssertionException(ISpace space)
            : base(space)
        {
        }
    }
}
