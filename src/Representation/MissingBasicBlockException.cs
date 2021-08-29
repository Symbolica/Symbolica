using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation
{
    [Serializable]
    public class MissingBasicBlockException : Exception
    {
        public MissingBasicBlockException(BasicBlockId id)
        {
            Id = id;
        }

        public BasicBlockId Id { get; }
    }
}
