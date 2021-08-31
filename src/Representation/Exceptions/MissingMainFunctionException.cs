using System;
using Symbolica.Expression;

namespace Symbolica.Representation.Exceptions
{
    [Serializable]
    public class MissingMainFunctionException : ErrorException
    {
        public MissingMainFunctionException()
            : base("No 'main' function is defined.")
        {
        }
    }
}
