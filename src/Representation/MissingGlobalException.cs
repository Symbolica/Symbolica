using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation
{
    [Serializable]
    public class MissingGlobalException : Exception
    {
        public MissingGlobalException(GlobalId id)
        {
            Id = id;
        }

        public GlobalId Id { get; }
    }
}
