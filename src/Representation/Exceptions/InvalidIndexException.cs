using System;
using Symbolica.Expression;

namespace Symbolica.Representation.Exceptions;

[Serializable]
public class InvalidIndexException : ErrorException
{
    public InvalidIndexException()
        : base("Indexing into single value type is invalid.")
    {
    }
}
