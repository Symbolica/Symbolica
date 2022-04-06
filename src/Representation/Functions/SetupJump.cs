using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression.Values.Constants;

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

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.Get(0);

        state.Stack.Save(address, true);

        state.Stack.SetVariable(caller.Id, ConstantUnsigned.CreateZero(caller.Size));
    }
}
