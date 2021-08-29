using System;
using Symbolica.Representation;

namespace Symbolica.Deserialization
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
