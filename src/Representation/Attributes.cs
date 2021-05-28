using Symbolica.Abstraction;

namespace Symbolica.Representation
{
    public sealed class Attributes : IAttributes
    {
        public Attributes(bool isSignExtended)
        {
            IsSignExtended = isSignExtended;
        }

        public bool IsSignExtended { get; }
    }
}
