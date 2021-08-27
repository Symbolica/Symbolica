using System;

namespace Symbolica.Computation
{
    public class ComputationException : Exception
    {
        public ComputationException(string message)
            : base(message)
        {
        }
    }
}
