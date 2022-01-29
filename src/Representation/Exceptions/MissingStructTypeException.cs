using System;
using Symbolica.Expression;

namespace Symbolica.Representation.Exceptions;

[Serializable]
public class MissingStructTypeException : ErrorException
{
    public MissingStructTypeException(string name)
        : base($"Struct type {name} was not found.")
    {
        Name = name;
    }

    public string Name { get; }
}
