using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class StackSave : IFunction
{
    public StackSave(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var address = state.Stack.Allocate(state.Memory, Bytes.One.ToBits());

        state.Stack.Save(state.Space, state.Memory, address, false);

        state.Stack.SetVariable(caller.Id, address);
    }
}
