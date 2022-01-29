using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Exceptions;

[Serializable]
public class MissingBasicBlockException : ErrorException
{
    public MissingBasicBlockException(BasicBlockId id)
        : base($"Basic block {id} was not found.")
    {
        Id = id;
    }

    public BasicBlockId Id { get; }
}
