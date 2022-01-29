using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions;

internal sealed class CountOnes : IFunction
{
    public CountOnes(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var expression = arguments.Get(0);

        var result = Enumerable.Range(0, (int) (uint) expression.Size)
            .Aggregate(state.Space.CreateConstant(expression.Size, BigInteger.Zero), (l, r) =>
                l.Add(expression.LogicalShiftRight(state.Space.CreateConstant(expression.Size, r))
                    .And(state.Space.CreateConstant(expression.Size, BigInteger.One))));

        state.Stack.SetVariable(caller.Id, result);
    }
}
