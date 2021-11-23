using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation
{
    [Serializable]
    public sealed class Attributes : IAttributes
    {
        public Attributes(bool isSignExtended)
        {
            IsSignExtended = isSignExtended;
        }

        public bool IsSignExtended { get; }
    }
}
