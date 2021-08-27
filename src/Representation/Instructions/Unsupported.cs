using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
    public sealed class Unsupported : IInstruction
    {
        private readonly string _type;

        public Unsupported(InstructionId id, string type)
        {
            Id = id;
            _type = type;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            throw new RepresentationException($"Instruction '{_type}' is unsupported.");
        }
    }
}
