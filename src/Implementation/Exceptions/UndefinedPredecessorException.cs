using System;
using Symbolica.Expression;

namespace Symbolica.Implementation.Exceptions
{
    [Serializable]
    public class UndefinedPredecessorException : ErrorException
    {
        public UndefinedPredecessorException()
            : base("Predecessor is undefined before transfer.")
        {
        }
    }
}
