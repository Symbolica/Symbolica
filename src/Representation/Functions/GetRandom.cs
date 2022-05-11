using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class GetRandom : IFunction
{
    public GetRandom(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.Get(0);
        var length = arguments.Get(1).GetSingleValue(state.Space);
        var size = (Bytes) (uint) length;

        state.Memory.Write(state.Space, address, exprFactory.CreateGarbage(size.ToBits()));
        state.Stack.SetVariable(caller.Id, exprFactory.CreateConstant(caller.Size, length));
    }
}
