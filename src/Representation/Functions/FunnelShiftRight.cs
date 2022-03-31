using Symbolica.Abstraction;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Functions;

internal sealed class FunnelShiftRight : IFunction
{
    public FunnelShiftRight(FunctionId id, IParameters parameters)
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
        var offset = Expression.Values.UnsignedRemainder.Create(shift, size);

        var result = Expression.Values.Or.Create(
            Expression.Values.LogicalShiftRight.Create(low, offset),
            Expression.Values.ShiftLeft.Create(high, Expression.Values.Subtract.Create(size, offset)));

        state.Stack.SetVariable(caller.Id, result);
    }
}
