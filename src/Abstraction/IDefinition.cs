namespace Symbolica.Abstraction
{
    public interface IDefinition : IFunction
    {
        string Name { get; }
        IBasicBlock Start { get; }

        IBasicBlock GetBasicBlock(BasicBlockId basicBlockId);
    }
}
