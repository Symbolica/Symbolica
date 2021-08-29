using System;

namespace Symbolica.Representation
{
    [Serializable]
    public class UnsupportedInstructionException : Exception
    {
        public UnsupportedInstructionException(string type)
        {
            Type = type;
        }

        public string Type { get; }
    }
}
