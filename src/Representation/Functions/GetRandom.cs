using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

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

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.GetAddress(0);
        var length = state.Space.GetSingleValue(arguments.Get(1));
        var size = (Bytes) (uint) length;

        state.Memory.Write(address, state.Space.CreateGarbage(size.ToBits()));
        state.Stack.SetVariable(caller.Id, ConstantUnsigned.Create(caller.Size, length));
    }
}
