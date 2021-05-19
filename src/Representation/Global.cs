using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    public sealed class Global : IGlobal
    {
        public Global(GlobalId id, Bits size, IOperand? initializer)
        {
            Id = id;
            Size = size;
            Initializer = initializer;
        }

        public GlobalId Id { get; }
        public Bits Size { get; }
        public IOperand? Initializer { get; }
    }
}