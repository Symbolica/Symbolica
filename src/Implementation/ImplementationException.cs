using System;

namespace Symbolica.Implementation
{
    [Serializable]
    public class ImplementationException : Exception
    {
        public ImplementationException(string message)
            : base(message)
        {
        }
    }
}
