using System;

namespace Symbolica.Implementation
{
    [Serializable]
    public class UnsupportedArchitectureException : Exception
    {
        public UnsupportedArchitectureException(string target)
        {
            Target = target;
        }

        public string Target { get; }
    }
}
