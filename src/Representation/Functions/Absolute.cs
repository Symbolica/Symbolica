using Symbolica.Abstraction;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Functions;

internal sealed class Absolute : IFunction
{
    public Absolute(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var value = arguments.Get(0);

        var mask = ArithmeticShiftRight.Create(
            value,
            ConstantUnsigned.Create(value.Size, (uint) value.Size - 1U));
        var result = Xor.Create(
            Add.Create(value, mask),
            mask);

        state.Stack.SetVariable(caller.Id, result);
    }
}
