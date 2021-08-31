using System;
using Symbolica.Expression;

namespace Symbolica.Representation.Exceptions
{
    [Serializable]
    public class MissingStructTypeException : SymbolicaException
    {
        public MissingStructTypeException(string name)
            : base($"Struct type {name} was not found.")
        {
            Name = name;
        }

        public string Name { get; }
    }
}
