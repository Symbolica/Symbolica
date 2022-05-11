using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class Close : IFunction
{
    public Close(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var descriptor = (int) arguments.Get(0).GetSingleValue(state.Space);

        var result = state.System.Close(descriptor);

        state.Stack.SetVariable(caller.Id, exprFactory.CreateConstant(caller.Size, result));
    }
}
