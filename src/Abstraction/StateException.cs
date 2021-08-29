using System;
using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    [Serializable]
    public class StateException : Exception
    {
        protected StateException(ISpace space)
        {
            Space = space;
        }

        public ISpace Space { get; }
    }
}
