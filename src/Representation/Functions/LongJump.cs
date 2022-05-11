using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

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

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.Get(0);
        var value = arguments.Get(1);

        var instructionId = state.Stack.Restore(state.Space, state.Memory, address, true);
        var result = value.Equal(exprFactory.CreateZero(value.Size))
            .Select(exprFactory.CreateConstant(value.Size, BigInteger.One), value);

        state.Stack.SetVariable(instructionId, result);
    }
}
