namespace Symbolica.Abstraction
{
    public interface IBasicBlock
    {
        BasicBlockId Id { get; }

        IInstruction GetInstruction(int index);
    }
}