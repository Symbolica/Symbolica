using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Exceptions
{
    [Serializable]
    public class MissingGlobalException : ErrorException
    {
        public MissingGlobalException(GlobalId id)
            : base($"Global {id} was not found.")
        {
            Id = id;
        }

        public GlobalId Id { get; }
    }
}
