using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class SetupJump : IFunction
{
    public SetupJump(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.Get(0);

        state.Stack.Save(state.Space, state.Memory, address, true);

        state.Stack.SetVariable(caller.Id, exprFactory.CreateZero(caller.Size));
    }
}
