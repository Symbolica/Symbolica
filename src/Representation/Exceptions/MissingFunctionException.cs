using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Exceptions
{
    [Serializable]
    public class MissingFunctionException : SymbolicaException
    {
        public MissingFunctionException(FunctionId id)
            : base($"Function {id} was not found.")
        {
            Id = id;
        }

        public FunctionId Id { get; }
    }
}
