using System;
using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    public class StateException : Exception
    {
        public StateException(string message, ISpace space)
            : base(message)
        {
            Space = space;
        }

        public ISpace Space { get; }
    }
}