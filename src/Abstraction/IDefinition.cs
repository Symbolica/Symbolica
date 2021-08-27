namespace Symbolica.Abstraction
{
    public interface IDefinition : IFunction
    {
        string Name { get; }
        IBasicBlock Entry { get; }

        IBasicBlock GetBasicBlock(BasicBlockId basicBlockId);
    }
}
