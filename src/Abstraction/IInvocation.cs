namespace Symbolica.Abstraction
{
    public interface IInvocation
    {
        IDefinition Definition { get; }
        IArguments Formals { get; }
        IArguments Varargs { get; }
    }
}