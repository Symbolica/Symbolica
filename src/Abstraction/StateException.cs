using System;
using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    public class StateException : Exception
    {
        public StateException(StateError error, ISpace space)
        {
            Error = error;
            Space = space;
        }

        public StateError Error { get; }
        public ISpace Space { get; }
    }
}
