using System;
using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    [Serializable]
    public class StateException : Exception
    {
        public StateException(StateError error, ISpace space)
            : base(error.ToString())
        {
            Error = error;
            Space = space;
        }

        public StateError Error { get; }
        public ISpace Space { get; }
    }
}
