using System;

namespace Symbolica.Representation
{
    [Serializable]
    public class UnsupportedOperandException : Exception
    {
        public UnsupportedOperandException(string type)
        {
            Type = type;
        }

        public string Type { get; }
    }
}
