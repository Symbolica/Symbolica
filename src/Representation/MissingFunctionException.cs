using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation
{
    [Serializable]
    public class MissingFunctionException : Exception
    {
        public MissingFunctionException(FunctionId id)
        {
            Id = id;
        }

        public FunctionId Id { get; }
    }
}
