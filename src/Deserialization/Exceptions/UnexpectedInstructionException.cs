using System;
using Symbolica.Representation.Exceptions;

namespace Symbolica.Deserialization.Exceptions
{
    [Serializable]
    public class UnexpectedInstructionException : UnsupportedInstructionException
    {
        public UnexpectedInstructionException(string type, string loweringPass)
            : base(type)
        {
            LoweringPass = loweringPass;
        }

        public string LoweringPass { get; }
    }
}
