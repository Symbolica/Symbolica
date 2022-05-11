using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class Seek : IFunction
{
    public Seek(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var descriptor = (int) arguments.Get(0).GetSingleValue(state.Space);
        var offset = (long) arguments.Get(1).GetSingleValue(state.Space);
        var whence = (uint) arguments.Get(2).GetSingleValue(state.Space);

        var result = state.System.Seek(descriptor, offset, whence);

        state.Stack.SetVariable(caller.Id, exprFactory.CreateConstant(caller.Size, result));
    }
}
