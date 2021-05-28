using Symbolica.Abstraction;

namespace Symbolica.Representation
{
    public sealed class BasicBlock : IBasicBlock
    {
        private readonly IInstruction[] _instructions;

        public BasicBlock(BasicBlockId id, IInstruction[] instructions)
        {
            Id = id;
            _instructions = instructions;
        }

        public BasicBlockId Id { get; }

        public IInstruction GetInstruction(int index)
        {
            return _instructions[index];
        }
    }
}
