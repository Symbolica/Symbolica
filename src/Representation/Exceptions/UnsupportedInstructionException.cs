using System;
using Symbolica.Expression;

namespace Symbolica.Representation.Exceptions
{
    [Serializable]
    public class UnsupportedInstructionException : ErrorException
    {
        public UnsupportedInstructionException(string type)
            : base($"Instruction '{type}' is unsupported.")
        {
            Type = type;
        }

        public string Type { get; }
    }
}
