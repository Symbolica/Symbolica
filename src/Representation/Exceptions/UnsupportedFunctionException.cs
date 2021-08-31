using System;
using Symbolica.Expression;

namespace Symbolica.Representation.Exceptions
{
    [Serializable]
    public class UnsupportedFunctionException : SymbolicaException
    {
        public UnsupportedFunctionException(string name)
            : base($"Function '{name}' is unsupported.")
        {
            Name = name;
        }

        public string Name { get; }
    }
}
