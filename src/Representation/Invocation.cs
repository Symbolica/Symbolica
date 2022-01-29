using Symbolica.Abstraction;

namespace Symbolica.Representation;

internal sealed class Invocation : IInvocation
{
    public Invocation(IDefinition definition, IArguments formals, IArguments varargs)
    {
        Definition = definition;
        Formals = formals;
        Varargs = varargs;
    }

    public IDefinition Definition { get; }
    public IArguments Formals { get; }
    public IArguments Varargs { get; }
}
