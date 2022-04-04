using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Functions;

internal sealed class LongJump : IFunction
{
    public LongJump(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.GetAddress(0);
        var value = arguments.Get(1);

        var instructionId = state.Stack.Restore(address, true);
        var result = Select.Create(
            Equal.Create(value, ConstantUnsigned.CreateZero(value.Size)),
            ConstantUnsigned.Create(value.Size, BigInteger.One),
            value);

        state.Stack.SetVariable(instructionId, result);
    }
}
