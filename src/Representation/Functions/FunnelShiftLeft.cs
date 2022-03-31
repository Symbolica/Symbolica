using Symbolica.Abstraction;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Functions;

internal sealed class FunnelShiftLeft : IFunction
{
    public FunnelShiftLeft(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var high = arguments.Get(0);
        var low = arguments.Get(1);
        var shift = arguments.Get(2);

        var size = ConstantUnsigned.Create(shift.Size, (uint) shift.Size);
        var offset = Expression.Values.UnsignedRemainder.Create(size, shift);

        var result = Expression.Values.Or.Create(
            Expression.Values.ShiftLeft.Create(high, offset),
            Expression.Values.LogicalShiftRight.Create(
                low,
                Expression.Values.Subtract.Create(size, offset)));

        state.Stack.SetVariable(caller.Id, result);
    }
}
